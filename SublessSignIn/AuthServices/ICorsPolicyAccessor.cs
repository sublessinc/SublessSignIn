using System.Collections.Generic;

namespace SublessSignIn.AuthServices
{
    public interface ICorsPolicyAccessor
    {
        void SetUnrestrictedOrigins();
    }
}