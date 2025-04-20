namespace SharpGameService.Core.Models
{
    /// <summary>
    /// Metadata for a room.
    /// </summary>
    public struct RoomMetadata
    {
        /// <summary>
        /// Gets or sets the max players in the room.
        /// </summary>
        public uint MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the current players in the room.
        /// </summary>
        public uint CurrentPlayers { get; set; }
    }
}
