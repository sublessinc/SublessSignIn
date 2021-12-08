using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.Models
{
    public static class UserExtensions
    {
        public static UserViewModel ToViewModel(this User user, string email)
        {
            return new UserViewModel
            {
                Email = email,
                Id = user.Id
            };
        }
    }
}
