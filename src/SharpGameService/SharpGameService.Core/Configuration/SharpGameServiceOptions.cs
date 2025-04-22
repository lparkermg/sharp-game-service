namespace SharpGameService.Core.Configuration
{
    /// <summary>
    /// Configuration options for the SharpGameService.
    /// </summary>
    public class SharpGameServiceOptions
    {
        /// <summary>
        /// Gets or sets the house options for the service.
        /// </summary>
        public HouseOptions House { get; set; } = new HouseOptions();

        /// <summary>
        /// Gets or sets the room options for the service.
        /// </summary>
        public RoomOptions Rooms { get; set; } = new RoomOptions();

        /// <summary>
        /// Gets or sets the maximum message size in kilobytes.
        /// </summary>
        public uint MaxMessageSizeKb { get; set; } = 4;        
    }

    public class HouseOptions
    {
        /// <summary>
        /// Gets or sets whether the service should run in single player mode.
        /// </summary>
        public bool SinglePlayer { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of rooms allowed.
        /// </summary>
        public uint MaxRooms { get; set; } = 1;

        /// <summary>
        /// Gets or sets the ticks per second for the room processing loop.
        /// </summary>
        public int TicksPerSecond { get; set; } = 120;
    }

    public class RoomOptions
    {
        /// <summary>
        /// Gets or sets the room Id for single player mode.
        /// </summary>
        public string SinglePlayerRoomId { get; set; } = "singleplayer";

        /// <summary>
        /// Gets or sets the room code for single player mode.
        /// </summary>
        public string SinglePlayerRoomCode { get; set; } = "singleplayer";
        
        /// <summary>
        /// Gets or sets the maximum players allowed in a room.
        /// </summary>
        public uint MaxPlayersPerRoom { get; set; } = 1;

        /// <summary>
        /// Gets or sets whether to close rooms when they are empty.
        /// </summary>
        public bool CloseRoomsOnEmpty { get; set; } = false;

        /// <summary>
        /// Gets or sets the time to wait before closing a room when it is empty.
        /// </summary>
        public TimeSpan? CloseWaitTime { get; set; } = null;
    }
}
