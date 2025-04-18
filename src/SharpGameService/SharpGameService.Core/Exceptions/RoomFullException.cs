using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Exceptions
{
    public class RoomFullException : Exception
    {
        public RoomFullException() : base("The room you're trying to enter is full")
        {
        }
    }
}
