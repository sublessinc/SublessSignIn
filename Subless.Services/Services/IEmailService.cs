using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface IEmailService
    {
        Task SendEmail(string body, string to, string subject, string from = "contact@subless.com");
    }
}
