using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class PaymentLogsService : IPaymentLogsService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;
        public PaymentLogsService(IUserRepository userRepository, ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = (loggerFactory?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger<PaymentLogsService>();
        }

        public void SaveLogs(IEnumerable<Payment> paymentLogs)
        {
            _userRepository.SavePaymentLogs(paymentLogs);
        }

        public void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs)
        {
            _userRepository.SavePaymentAuditLogs(paymentAuditLogs);
        }

        public DateTime GetLastPaymentDate()
        {
            var date = _userRepository.GetLastPaymentDate();
            _logger.LogInformation($"Last payment date {date}");
            return date;
        }
    }
}
