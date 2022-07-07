using Subless.Models;

namespace SublessSignIn.Models
{
    public static class UserExtensions
    {
        public static UserViewModel ToViewModel(this User user, string email)
        {
            return new UserViewModel
            {
                Email = email,
                Id = user?.Id
            };
        }
    }
}
