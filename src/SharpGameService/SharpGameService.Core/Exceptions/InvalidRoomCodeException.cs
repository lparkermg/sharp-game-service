namespace SharpGameService.Core.Exceptions
{
    /// <summary>
    /// Exception detailing an invalid room code.
    /// </summary>
    public class InvalidRoomCodeException : Exception
    {
        public InvalidRoomCodeException() : base("The room code provided is invalid")
        {
        }
    }
}
