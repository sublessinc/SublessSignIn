using System;

namespace Subless.PayoutCalculator
{
    public interface IPaymentService
    {
        void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate);
        void SaveFirstPayment();
    }
}
