using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public class CognitoWrapper : ICognitoWrapper
    {
        private readonly ICognitoService cognitoService;
        private readonly IUserService userService;

        public CognitoWrapper(
            ICognitoService cognitoService, 
            IUserService userService)
        {
            this.cognitoService = cognitoService;
            this.userService = userService;
        }

        public void SetEmail(string id)
        {
            var usertask = Task.Run(() => cognitoService.GetCognitoUserEmail(id));
            usertask.Wait();
            var user = userService.GetUserByCognitoId(id);
            user.Replica_Email = usertask.Result;
            userService.UpdateUser(user);
        }
    }
}
