using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Exceptions
{
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException() : base("The room for the provided Id does not exist")
        {
        }
    }
}
