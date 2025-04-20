namespace SharpGameService.Core.Exceptions
{
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException() : base("The room for the provided Id does not exist")
        {
        }
    }
}
