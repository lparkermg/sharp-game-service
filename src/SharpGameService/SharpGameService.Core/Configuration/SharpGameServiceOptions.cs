namespace SharpGameService.Core.Configuration
{
    public class SharpGameServiceOptions
    {
        public bool SinglePlayer { get; set; } = true;

        public int MaxPlayersPerRoom { get; set; } = 1;

        public int MaxRooms { get; set; } = 1;

        public int MaxMessageSizeKb { get; set; } = 4;
    }
}
