using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Events
{
    /// <summary>
    /// Event arguments for when a player disconnects from a room.
    /// </summary>
    /// <param name="playerId">The id of the player.</param>
    public class OnPlayerDisconnectedEventArgs(string playerId) : EventArgs
    {
        /// <summary>
        /// Gets the player id.
        /// </summary>
        public string PlayerId { get; private set; } = playerId;
    }
}
