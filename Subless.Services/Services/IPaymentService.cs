using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface IPaymentService
    {
        void ExecutedQueuedPayment();
        Task ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null);
        Task ExecuteQueuedIdleEmail();
        void QueueIdleEmail(DateTimeOffset start, DateTimeOffset end);
        Guid QueuePayment(DateTimeOffset startDate, DateTimeOffset endDate);
        void SaveFirstPayment();
    }
}
