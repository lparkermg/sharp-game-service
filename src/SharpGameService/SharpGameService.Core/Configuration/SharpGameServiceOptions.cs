namespace SharpGameService.Core.Configuration
{
    public class SharpGameServiceOptions
    {
        public bool SinglePlayer { get; set; } = true;

        public uint MaxPlayersPerRoom { get; set; } = 1;

        public uint MaxRooms { get; set; } = 1;

        public uint MaxMessageSizeKb { get; set; } = 4;
    }
}
