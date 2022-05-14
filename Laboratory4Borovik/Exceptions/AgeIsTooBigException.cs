using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratory4Borovik.Exceptions
{
    internal class AgeIsTooBigException : Exception
    {
        public AgeIsTooBigException(string message) : base(message)
        {
        }
    }
}
