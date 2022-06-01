using System;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface ICalculatorService
    {
        CalculatorResult CaculatePayoutsOverRange(DateTimeOffset startDate, DateTimeOffset endDate);
        void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate);
        void SaveFirstPayment();
    }
}
