using System;
using System.Collections.Generic;

namespace Subless.Services.Services
{
    public interface IPaymentService
    {
        void ExecutedQueuedPayment();
        void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null);
        Guid QueuePayment(DateTimeOffset startDate, DateTimeOffset endDate);
        void SaveFirstPayment();
    }
}
