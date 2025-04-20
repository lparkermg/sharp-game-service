namespace SharpGameService.Core.Models.Requests
{
    /// <summary>
    /// Request model used for creating a new game room.
    /// </summary>
    public class CreateRoomRequest
    {
        /// <summary>
        /// Gets or sets the ID of the room to be created.
        /// </summary>
        public required string RoomId { get; set; }

        /// <summary>
        /// Gets or sets the code used for the room.
        /// </summary>
        public string? RoomCode { get; set; } = null;
    }
}
