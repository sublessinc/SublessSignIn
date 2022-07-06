using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IPaymentEmailService
    {
        string GetEmailBody(List<Payment> payments, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd);
        void SendAdminNotification();
        void SendCreatorReceiptEmail(Guid id, PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd);
        void SendPartnerReceiptEmail(Guid id, PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd);
        void SendPatronRolloverReceiptEmail(string cognitoId, double payment, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd);
        void SendReceiptEmail(List<Payment> payments, string cognitoId, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd);
    }
}
