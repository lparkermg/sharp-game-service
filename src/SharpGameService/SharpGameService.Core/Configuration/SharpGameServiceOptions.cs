using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
