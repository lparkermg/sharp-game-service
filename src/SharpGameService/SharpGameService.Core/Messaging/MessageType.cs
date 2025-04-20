using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Messaging
{
    public enum MessageType
    {
        // in house actions
        JOIN_ROOM = 0,
        ROOM_JOINED = 1,
        DISCONNECT_ROOM = 2,
        // in room actions
        PERFORM_ACTION = 100,
        // responses
        STATE_UPDATED = 200,
    }
}
