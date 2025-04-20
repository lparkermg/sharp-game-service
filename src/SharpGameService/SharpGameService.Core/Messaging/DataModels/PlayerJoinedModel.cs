using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Messaging.DataModels
{
    public sealed class PlayerJoinedModel
    {
        public required string PlayerName { get; set; }

        public required string PlayerId { get; set; }
    }
}
