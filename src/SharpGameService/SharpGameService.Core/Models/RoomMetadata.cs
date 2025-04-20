using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Models
{
    public struct RoomMetadata
    {
        public uint MaxPlayers { get; set; }

        public uint CurrentPlayers { get; set; }
    }
}
