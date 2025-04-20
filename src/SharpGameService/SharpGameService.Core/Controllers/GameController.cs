using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpGameService.Core.Configuration;
using SharpGameService.Core.Interfaces;
using SharpGameService.Core.Messaging;
using SharpGameService.Core.Messaging.DataModels;
using SharpGameService.Core.Models.Requests;
using System.Net.WebSockets;
using System.Text.Json;

namespace SharpGameService.Core.Controllers
{
    /// <summary>
    /// Controller for managing game rooms.
    /// </summary>
    /// <param name="logger">The services logger.</param>
    /// <param name="options">The options for the service.</param>
    /// <param name="house">The house users are connecting to.</param>
    public class GameController(ILogger<GameController> logger, IOptions<SharpGameServiceOptions> options, IHouse house) : ControllerBase
    {
        /// <summary>
        /// The endpoint for creating a room.
        /// </summary>
        /// <param name="body">The body of the request containing the room id and code to setup with.</param>
        /// <returns>The result of the action.</returns>
        [HttpPost("/game")]
        public IActionResult CreateRoom([FromBody] CreateRoomRequest body)
        {
            if (string.IsNullOrWhiteSpace(body.RoomId))
            {
                logger.LogError("Room ID is required.");
                return BadRequest("Room ID is required.");
            }

            if (string.IsNullOrWhiteSpace(body.RoomCode))
            {
                logger.LogError("Room code is required.");
                return BadRequest("Room code is required.");
            }

            if (house.DoesRoomExist(body.RoomId))
            {
                logger.LogWarning("Room already exists: {roomId}", body.RoomId);
                return Conflict("Room already exists.");
            }

            try
            {
                house.CreateRoom(body.RoomId, body.RoomCode);
                logger.LogInformation("Room created successfully: {roomId}", body.RoomId);
                return Created($"/game/{body.RoomId}", new object());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create room: {roomId}", body.RoomId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create room.");
            }
        }

        /// <summary>
        /// The route for connecting to a game room.
        /// </summary>
        /// <param name="gameRoomId">The id of the room.</param>
        /// <returns>A task representing an async operation.</returns>
        [Route("/game/{gameRoomId}")]
        public async Task Get([FromRoute] string gameRoomId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var success = house.DoesRoomExist(gameRoomId);
                if (!success)
                {
                    logger.LogWarning("Room not found: {gameRoomId}", gameRoomId);
                    await HttpContext.Response.WriteAsync("Room not found.");
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    logger.LogInformation("Connection established for room: {gameRoomId}", gameRoomId);
                    await ProcessIncomingMessagesAsync(socket, gameRoomId);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                HttpContext.Response.ContentType = "text/plain";
                await HttpContext.Response.WriteAsync("WebSocket connection required.");
            }
        }

        private async Task ProcessIncomingMessagesAsync(WebSocket webSocket, string providedRoomId)
        {
            var buffer = new byte[1024 * options.Value.MaxMessageSizeKb];
            var msgResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!webSocket.CloseStatus.HasValue)
            {
                await ProcessMessageAsync(buffer, webSocket, providedRoomId);
                msgResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(msgResult.CloseStatus.Value, msgResult.CloseStatusDescription, CancellationToken.None);
        }

        private async Task ProcessMessageAsync(byte[] buffer, WebSocket socket, string providedRoomId)
        {
            using var stream = new MemoryStream(buffer);
            var message = await JsonSerializer.DeserializeAsync<Message>(stream);

            if (message == null)
            {
                logger.LogError("Message deserialization failed.");
                return;
            }

            switch (message.Type)
            {
                case MessageType.JOIN_ROOM:
                    var joinData = JsonSerializer.Deserialize<JoinRoomModel>(message.Data);

                    if (joinData == null)
                    {
                        logger.LogError("Join room data deserialization failed.");
                        break;
                    }

                    house.Join(providedRoomId, joinData.RoomCode, joinData.PlayerName, socket);
                    break;
                case MessageType.DISCONNECT_ROOM:
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "DISCONNECT_ROOM message received", CancellationToken.None);
                    break;
                case MessageType.PERFORM_ACTION:
                    var actionData = JsonSerializer.Deserialize<PerformActionModel>(message.Data);

                    if (actionData == null)
                    {
                        logger.LogError("Perform action data deserialization failed.");
                        break;
                    }

                    house.MessageReceived(providedRoomId, actionData.ActionData);
                    break;
                default:
                    logger.LogError("Unknown message type: {messageType}", message.Type);
                    break;
            }
        }
    }
}