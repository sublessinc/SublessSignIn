using Subless.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface IPaymentEmailService
    {
        string GetEmailBody(List<Payment> payments);
        void SendAdminNotification();
        void SendReceiptEmail(List<Payment> payments, string cognitoId);
    }
}
