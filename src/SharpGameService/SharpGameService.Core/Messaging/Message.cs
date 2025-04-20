using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Messaging
{
    /// <summary>
    /// Message class used to send messages between the client and server.
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        /// Gets or sets the type of message.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Gets or sets the data of the message.
        /// </summary>
        public required string Data { get; set; }
    }
}
