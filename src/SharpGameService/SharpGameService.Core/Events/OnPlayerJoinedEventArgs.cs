namespace SharpGameService.Core.Events
{
    // TOOD: Might change this to be a typed object instead of a generic object to include certain required details.
    /// <summary>
    /// Event arguments for when a player joins a room.
    /// </summary>
    /// <param name="playerDetails">The player details object.</param>
    public class OnPlayerJoinedEventArgs(object playerDetails)
    {
        /// <summary>
        /// Gets the player details object of the event.
        /// </summary>
        public object PlayerDetails { get; private set; } = playerDetails;
    }
}
