using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Messaging.DataModels
{
    /// <summary>
    /// Data model used for joining a room.
    /// </summary>
    public sealed class JoinRoomModel
    {
        /// <summary>
        /// Gets or sets the Name of the player joining the room.
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the Id of the room to join
        /// </summary>
        public string RoomId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional code of the room to join.
        /// </summary>
        public string? RoomCode { get; set; }
    }
}
