using System;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public interface IUserRepository
    {
        Guid AddUser(User user);
        User GetUserByStripeId(string id);
        void UpdateUser(User user);
        User GetUserByCognitoId(string id);
    }
}