using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Data
{
    public interface ICalculatorQueueRepository
    {
        void CompleteCalculation(CalculatorExecution calculatorExecution);
        void CompletePayment(PaymentExecution payment);
        CalculatorExecution DequeueCalculation();
        PaymentExecution DequeuePayment();
        Guid QueueCalculation(DateTimeOffset start, DateTimeOffset end);
        Guid QueuePayment(DateTimeOffset start, DateTimeOffset end);
        CalculatorExecution GetQueuedCalcuation(Guid id);
    }
}
