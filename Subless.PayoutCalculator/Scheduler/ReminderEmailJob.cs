using Subless.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.PayoutCalculator.Scheduler
{
    public class ReminderEmailJob : IReminderEmailJob
    {
        private readonly IPaymentService paymentService;

        public ReminderEmailJob(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }
        public void QueueIdleEmailsForThisMonth()
        {
            var now = DateTime.Now;
            var start = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var end = new DateTimeOffset(now.Year, now.Month + 1, 1, 0, 0, 0, TimeSpan.Zero);
            paymentService.QueueIdleEmail(start, end);
        }
    }
}
