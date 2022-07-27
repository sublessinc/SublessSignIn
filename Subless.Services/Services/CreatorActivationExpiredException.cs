using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public class CreatorActivationExpiredException : Exception
    {
        override public string Message => "The creator activation code is expired";
    }
}
