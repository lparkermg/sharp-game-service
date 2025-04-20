namespace SharpGameService.Core.Events
{
    /// <summary>
    /// Event arguments for when a message is received for a room.
    /// </summary>
    /// <param name="data">The data to be processed by the room.</param>
    public class OnMessageReceivedEventArgs(string data) : EventArgs
    {
        /// <summary>
        /// Gets the data object of the event.
        /// </summary>
        public string Data { get; private set; } = data;
    }
}
