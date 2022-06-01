using System;

namespace Subless.Services.Services
{
    public interface ICalculatorService
    {
        void CalculatePayments(DateTimeOffset startDate, DateTimeOffset endDate);
        void SaveFirstPayment();
    }
}
