namespace SharpGameService.Core.Exceptions
{
    /// <summary>
    /// Exception detailing a house being full.
    /// </summary>
    public class HouseFullException : Exception
    {
        public HouseFullException() : base("The house you are trying to enter is full")
        {
        }
    }
}
