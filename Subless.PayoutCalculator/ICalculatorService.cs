using System;

namespace Subless.PayoutCalculator
{
    public interface ICalculatorService
    {
        void CalculatePayments(DateTimeOffset startDate, DateTimeOffset endDate);
    }
}