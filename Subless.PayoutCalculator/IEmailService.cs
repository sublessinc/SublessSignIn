using Subless.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface IEmailService
    {
        string GetEmailBody(List<Payment> payments);
        Task SendEmail(string body, string to, string from, string subject);
        void SendReceiptEmail(List<Payment> payments, string cognitoId);
    }
}