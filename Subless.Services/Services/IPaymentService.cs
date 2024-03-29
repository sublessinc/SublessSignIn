using System;
using System.Collections.Generic;

namespace Subless.Services.Services
{
    public interface IPaymentService
    {
        void ExecutedQueuedPayment();
        void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null);
        void ExecuteQueuedIdleEmail();
        void ExecuteStripeSync();
        void QueueIdleEmail(DateTimeOffset start, DateTimeOffset end);
        Guid QueuePayment(DateTimeOffset startDate, DateTimeOffset endDate);
        void QueueStripeSync();
        void SaveFirstPayment();
    }
}
