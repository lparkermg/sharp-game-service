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
        int MaxPlayers { get; }

        /// <summary>
        /// Gets the Id of the room.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the code of the room.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Initialises the room 
        /// </summary>
        /// <param name="roomId">Id of the room.</param>
        /// <param name="roomCode">Code of the room.</param>
        /// <param name="maxPlayers">Max players allowed in the room.</param>
        /// <param name="dataFilePath">The path where data is loaded and saved.</param>
        void Initialise(string roomId, string roomCode, int maxPlayers, string dataFilePath);

        /// <summary>
        /// Joins the room with the specified connection.
        /// </summary>
        /// <param name="connection">The players connection.</param>
        void Join(WebSocket connection);

        /// <summary>
        /// Processes and automatic details of the room
        /// </summary>
        /// <returns>A task representing </returns>
        Task Process();
    }
}
