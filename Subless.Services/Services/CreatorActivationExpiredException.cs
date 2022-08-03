using System;

namespace Subless.Services.Services
{
    public class CreatorActivationExpiredException : Exception
    {
        override public string Message => "The creator activation code is expired";
    }
}
