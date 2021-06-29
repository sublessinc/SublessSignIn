using System;
using System.Collections.Generic;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class PaymentLogsService : IPaymentLogsService
    {
        private readonly IUserRepository _userRepository;

        public PaymentLogsService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public void SaveLogs(IEnumerable<Payment> paymentLogs)
        {
            _userRepository.SavePaymentLogs(paymentLogs);
        }

        public void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs)
        {
            _userRepository.SavePaymentAuditLogs(paymentAuditLogs);
        }
    }
}
