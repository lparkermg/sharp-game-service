using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;

namespace SharpGameService.Core.Controllers
{
    public class GameController(ILogger<GameController> logger) : ControllerBase
    {
        [Route("/game/{gameRoomId}")]
        public async void Get([FromRoute] string gameRoomId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                // TOOD: Handle trying to join a room.
                var success = false;
                if (!success)
                {
                    logger.LogWarning("Room not found: {gameRoomId}", gameRoomId);
                    await HttpContext.Response.WriteAsync("Room not found.");
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    logger.LogInformation("Connection established for room: {gameRoomId}", gameRoomId);
                    // TODO: Send message back to client that they are connected.
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

        private async Task ProcessIncomingMessagesAsync(WebSocket webSocket, string roomId)
        {
            bool roomExists = false;

            if (roomExists)
            {
                logger.LogError("Room ({gameRoomId}) could not be found", roomId);
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Room not found.", CancellationToken.None);
            }

            var buffer = new byte[1024 * 4];
            var msgResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!webSocket.CloseStatus.HasValue)
            {
                // TODO: Send message to room.
                msgResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(msgResult.CloseStatus.Value, msgResult.CloseStatusDescription, CancellationToken.None);
        }
    }
}