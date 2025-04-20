using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Models.Requests
{
    public class CreateRoomRequest
    {
        public required string RoomId { get; set; }

        public string? RoomCode { get; set; } = null;
    }
}
