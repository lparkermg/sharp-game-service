namespace SharpGameService.Core.Exceptions
{
    public class InvalidRoomCodeException : Exception
    {
        public InvalidRoomCodeException() : base("The room code provided is invalid")
        {
        }
    }
}
