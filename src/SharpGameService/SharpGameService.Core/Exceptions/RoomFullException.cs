namespace SharpGameService.Core.Exceptions
{
    public class RoomFullException : Exception
    {
        public RoomFullException() : base("The room you're trying to enter is full")
        {
        }
    }
}
