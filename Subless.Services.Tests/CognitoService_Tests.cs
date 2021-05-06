using System;
using Xunit;

namespace Subless.Services.Tests
{
    public class CognitoService_Tests
    {
        [Fact]
        public void GetAuthData()
        {
            var client = new CognitoClient();
            client.GetLoginEvents("google_112349431240835854597");

        }
    }
}
