namespace SharpGameService.Core.Messaging.DataModels
{
    /// <summary>
    /// Data model used for when a player joins a room.
    /// </summary>
    public sealed class PlayerJoinedModel
    {
        /// <summary>
        /// Gets or sets the players name.
        /// </summary>
        public required string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the players Id.
        /// </summary>
        public required string PlayerId { get; set; }
    }
}
