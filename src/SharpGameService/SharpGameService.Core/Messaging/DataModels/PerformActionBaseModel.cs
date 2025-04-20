namespace SharpGameService.Core.Messaging.DataModels
{
    /// <summary>
    /// Data model used for performing an action in a game room.
    /// </summary>
    public sealed class PerformActionModel
    {
        /// <summary>
        /// Gets or sets the action data being performed.
        /// </summary>
        public required string ActionData { get; set; }
    }
}
