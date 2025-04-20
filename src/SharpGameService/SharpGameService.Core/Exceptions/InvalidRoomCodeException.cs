using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Exceptions
{
    public class InvalidRoomCodeException : Exception
    {
        public InvalidRoomCodeException() : base("The room code provided is invalid")
        {
        }
    }
}
