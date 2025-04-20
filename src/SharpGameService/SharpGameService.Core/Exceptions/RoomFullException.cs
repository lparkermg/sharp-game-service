namespace SharpGameService.Core.Exceptions
{
    /// <summary>
    /// Exception detailing a room being full.
    /// </summary>
    public class RoomFullException : Exception
    {
        public RoomFullException() : base("The room you're trying to enter is full")
        {
        }
    }
}
