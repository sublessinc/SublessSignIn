using System;
using System.Collections.Generic;

namespace Subless.Services.Services
{
    public interface IPaymentService
    {
        void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null);
        void SaveFirstPayment();
    }
}
