using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;

namespace Subless.Services.Services
{
    public class PaymentLogsService : IPaymentLogsService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly ILogger _logger;
        public PaymentLogsService(
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger<PaymentLogsService>();
        }

        public void SaveLogs(IEnumerable<Payment> paymentLogs)
        {
            paymentRepository.SavePaymentLogs(paymentLogs);
        }

        public void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs)
        {
            paymentRepository.SavePaymentAuditLogs(paymentAuditLogs);
        }

        public DateTimeOffset GetLastPaymentDate()
        {
            var date = paymentRepository.GetLastPaymentDate();
            _logger.LogInformation($"Last payment date {date}, time now {DateTimeOffset.UtcNow}");
            return date;
        }
    }
}
