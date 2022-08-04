using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public partial class Repository : DbContext, ICalculatorQueueRepository
    {
        internal DbSet<CalculatorExecution> CalculatorExecutions { get; set; }
        internal DbSet<PaymentExecution> PaymentExecutions { get; set; }
        internal DbSet<IdleEmailExecution> IdleEmailExecutions { get; set; }

        public Guid QueueCalculation(DateTimeOffset start, DateTimeOffset end)
        {
            var calculatorExecution = new CalculatorExecution
            {
                PeriodStart = start,
                PeriodEnd = end,
                DateQueued = DateTime.UtcNow,
            };
            CalculatorExecutions.Add(calculatorExecution);
            SaveChanges();
            return calculatorExecution.Id;
        }

        public CalculatorExecution DequeueCalculation()
        {
            var calculation = CalculatorExecutions.Where(x => x.IsProcessing == false && x.IsCompleted == false).OrderBy(x => x.DateQueued).FirstOrDefault();
            if (calculation == null)
            {
                return null;
            }
            calculation.IsProcessing = true;
            CalculatorExecutions.Update(calculation);
            SaveChanges();
            return calculation;
        }

        public void CompleteCalculation(CalculatorExecution calculatorExecution)
        {
            calculatorExecution.DateExecuted = DateTime.UtcNow;
            calculatorExecution.IsCompleted = true;
            CalculatorExecutions.Update(calculatorExecution);
            SaveChanges();
        }

        public Guid QueuePayment(DateTimeOffset start, DateTimeOffset end)
        {
            var payment = new PaymentExecution
            {
                PeriodStart = start,
                PeriodEnd = end,
                DateQueued = DateTime.UtcNow,
            };
            PaymentExecutions.Add(payment);
            SaveChanges();
            return payment.Id;
        }

        public PaymentExecution DequeuePayment()
        {
            var payment = PaymentExecutions.Where(x => x.IsProcessing == false && x.IsCompleted == false).OrderBy(x => x.DateQueued).FirstOrDefault();
            if (payment == null)
            {
                return null;
            }
            payment.IsProcessing = true;
            PaymentExecutions.Update(payment);
            SaveChanges();
            return payment;
        }

        public void CompletePayment(PaymentExecution payment)
        {
            payment.DateExecuted = DateTime.UtcNow;
            payment.IsCompleted = true;
            PaymentExecutions.Update(payment);
            SaveChanges();
        }

        public CalculatorExecution GetQueuedCalcuation(Guid id)
        {
            return CalculatorExecutions.SingleOrDefault(x => x.Id == id && x.IsCompleted);
        }

        public void QueueIdleEmails(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var emails = new IdleEmailExecution
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                DateQueued = DateTime.UtcNow,
            };
            IdleEmailExecutions.Add(emails);
            SaveChanges();
        }

        public IdleEmailExecution DequeueIdleEmails()
        {
            var emails = IdleEmailExecutions.Where(x => x.IsProcessing == false && x.IsCompleted == false).OrderBy(x => x.DateQueued).FirstOrDefault();
            if (emails == null)
            {
                return null;
            }
            emails.IsProcessing = true;
            IdleEmailExecutions.Update(emails);
            SaveChanges();
            return emails;
        }

        public void CompleteIdleEmails(IdleEmailExecution email)
        {
            email.DateExecuted = DateTime.UtcNow;
            email.IsCompleted = true;
            IdleEmailExecutions.Update(email);
            SaveChanges();
        }
    }
}
