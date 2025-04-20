namespace SharpGameService.Core.Exceptions
{
    /// <summary>
    /// Exception detailing a room not being found.
    /// </summary>
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException() : base("The room for the provided Id does not exist")
        {
        }
    }
}
