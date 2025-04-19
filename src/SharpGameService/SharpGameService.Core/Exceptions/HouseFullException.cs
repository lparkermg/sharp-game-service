using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Exceptions
{
    public class HouseFullException : Exception
    {
        public HouseFullException() : base("The house you are trying to enter is full")
        {
        }
    }
}
