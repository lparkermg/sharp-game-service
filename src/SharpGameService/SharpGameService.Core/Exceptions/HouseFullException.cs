namespace SharpGameService.Core.Exceptions
{
    public class HouseFullException : Exception
    {
        public HouseFullException() : base("The house you are trying to enter is full")
        {
        }
    }
}
