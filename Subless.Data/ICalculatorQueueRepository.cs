using System;
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
        IdleEmailExecution DequeueIdleEmails();
        void QueueIdleEmails(DateTimeOffset startDate, DateTimeOffset endDate);
        void CompleteIdleEmails(IdleEmailExecution payment);
        void CompletsStripeSync(StripeSync sync);
        StripeSync DequeueStripeSync();
        void QueueStripeSync();
    }
}
