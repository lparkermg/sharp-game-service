using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharpGameService.Core.Interfaces;
using SharpGameService.Core.Messaging;
using SharpGameService.Core.Messaging.DataModels;
using System.Net.WebSockets;
using System.Text.Json;

namespace SharpGameService.Core.Controllers
{
    public class GameController(ILogger<GameController> logger, IHouse house) : ControllerBase
    {
        [Route("/game/{gameRoomId}")]
        public async void Get([FromRoute] string gameRoomId)
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
                HttpContext.Response.WriteAsync("WebSocket connection required.");
            }
        }

        private async Task ProcessIncomingMessagesAsync(WebSocket webSocket, string providedRoomId)
        {
            var buffer = new byte[1024 * 4];
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