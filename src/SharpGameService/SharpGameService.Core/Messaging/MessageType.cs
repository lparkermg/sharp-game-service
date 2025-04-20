namespace SharpGameService.Core.Messaging
{
    /// <summary>
    /// Enumeration representing the different types of messages that can be sent or recieved.
    /// </summary>
    public enum MessageType
    {
        // in house actions
        JOIN_ROOM = 0,
        ROOM_JOINED = 1,
        DISCONNECT_ROOM = 2,
        // in room actions
        PERFORM_ACTION = 100,
        // responses
        STATE_UPDATED = 200,
    }
}
