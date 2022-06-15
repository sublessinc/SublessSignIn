using Subless.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface IPaymentEmailService
    {
        string GetEmailBody(List<Payment> payments);
        void SendAdminNotification();
        void SendCreatorReceiptEmail(Guid id, PaymentAuditLog paymentAuditLog);
        void SendPartnerReceiptEmail(Guid id, PaymentAuditLog paymentAuditLog);
        void SendReceiptEmail(List<Payment> payments, string cognitoId);
    }
}
