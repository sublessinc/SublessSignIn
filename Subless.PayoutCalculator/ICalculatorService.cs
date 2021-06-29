using System;

namespace Subless.PayoutCalculator
{
    public interface ICalculatorService
    {
        void CalculatePayments(DateTime startDate, DateTime endDate);
    }
}