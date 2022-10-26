using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class InputInvalidException : Exception
    {
        public InputInvalidException(string message) : base(message)
        {
        }
    }
}
