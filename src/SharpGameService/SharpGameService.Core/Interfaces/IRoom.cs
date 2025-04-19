using System.Net.WebSockets;

namespace SharpGameService.Core.Interfaces
{
    /// <summary>
    /// Interface for game rooms which handles connections to the game
    /// </summary>
    public interface IRoom
    {
        /// <summary>
        /// Gets the maximum number of players in the room.
        /// </summary>
        uint MaxPlayers { get; }

        /// <summary>
        /// Gets the current number of players in the room.
        /// </summary>
        uint CurrentPlayers { get; }

        /// <summary>
        /// Gets the Id of the room.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the code of the room.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets whether the room is closing or not.
        /// </summary>
        bool RoomClosing { get; }

        /// <summary>
        /// Initialises the room 
        /// </summary>
        /// <param name="roomId">Id of the room.</param>
        /// <param name="roomCode">Code of the room.</param>
        /// <param name="maxPlayers">Max players allowed in the room.</param>
        /// <param name="closeOnEmpty">Whether the room should close when empty.</param>
        /// <param name="closeWaitTime">Time to wait before closing the room.</param>
        void Initialise(string roomId, string roomCode, uint maxPlayers, bool closeOnEmpty, TimeSpan? closeWaitTime = null);

        /// <summary>
        /// Joins the room with the specified connection.
        /// </summary>
        /// <param name="connection">The players connection.</param>
        void Join(WebSocket connection);

        /// <summary>
        /// Processes and automatic details of the room.
        /// </summary>
        /// <returns>A task representing an async operation.</returns>
        Task Process();

        /// <summary>
        /// Handles messages received outside of the room (usually by the websocket connection). 
        /// </summary>
        /// <param name="data">The message data for the implemented room to process.</param>
        void HandleReceivedMessage(object data);
    }
}
