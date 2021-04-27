using Microsoft.EntityFrameworkCore;

namespace Subless.Data
{
    public interface IUserRepository
    {
        DbSet<User> Users { get; set; }

        void AddUser(User user);
        User GetUserByStripeId(string id);
        void UpdateUser(User user);
    }
}