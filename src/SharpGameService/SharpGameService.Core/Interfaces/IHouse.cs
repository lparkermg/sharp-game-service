using SharpGameService.Core.Models;
using System.Net.WebSockets;

namespace SharpGameService.Core.Interfaces
{
    /// <summary>
    /// Interface for the House which handles high-level room management.
    /// </summary>
    public interface IHouse
    {
        /// <summary>
        /// Creates a room with the specified ID and code.
        /// </summary>
        /// <param name="roomId">Id of the room</param>
        /// <param name="roomCode">Code of the room, if any.</param>
        void CreateRoom(string roomId, string roomCode);

        /// <summary>
        /// Joins a room with the specified ID and code.
        /// </summary>
        /// <param name="roomdId">Id of the room.</param>
        /// <param name="code">Code of the room.</param>
        /// <param name="connection">The users connection.</param>
        void Join(string roomdId, string code, WebSocket connection);

        /// <summary>
        /// Checks whether the room with the specified ID exists.
        /// </summary>
        /// <param name="roomId">Id of the room.</param>
        /// <returns>Whether the room exists or not.</returns>
        bool DoesRoomExist(string roomId);

        /// <summary>
        /// Processes the automatic functionality of all current rooms.
        /// </summary>
        /// <returns>A task representing an async operation.</returns>
        Task ProcessAsync();

        /// <summary>
        /// Gets the metadata of a room with the specified ID.
        /// </summary>
        /// <param name="roomId">The Id of the room.</param>
        /// <returns>The <see cref="RoomMetadata"/> of the room.</returns>
        RoomMetadata GetRoomMetadata(string roomId);

        /// <summary>
        /// Handles messages received outside of the room (usually by the websocket connection).
        /// </summary>
        /// <param name="roomId">Id of the room</param>
        /// <param name="message">Message data.</param>
        /// <returns></returns>
        void MessageReceived(string roomId, string message);
    }
}
