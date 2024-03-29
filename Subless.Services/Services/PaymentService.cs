﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Data;
using Subless.Models;
using Subless.Services.Services.SublessStripe;

namespace Subless.Services.Services
{
    public class PaymentService : IPaymentService
    {
        //TODO, these need to be configurable or something
        public const double PartnerFraction = .2;
        public const double SublessFraction = .02;
        public readonly string SublessPayPalId;
        public const int CurrencyPrecision = 4;
        private readonly IStripeService _stripeService;
        private readonly IPaymentLogsService _paymentLogsService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFileStorageService _s3Service;
        private readonly ITemplatedEmailService emailService;
        private readonly ICalculatorService _calculatorService;
        private readonly ICalculatorQueueRepository _calculationQueueRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public PaymentService(
            IStripeService stripeService,
            IPaymentLogsService paymentLogsService,
            IFileStorageService s3Service,
            IOptions<StripeConfig> stripeOptions,
            ITemplatedEmailService emailService,
            ICalculatorService calculatorService,
            ICalculatorQueueRepository calculationQueueRepository,
            IUserService userService,
            ILoggerFactory loggerFactory)
        {
            if (stripeOptions is null)
            {
                throw new ArgumentNullException(nameof(stripeOptions));
            }

            _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            _paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
            _calculationQueueRepository = calculationQueueRepository;
            this._userService = userService;
            _logger = _loggerFactory.CreateLogger<PaymentService>();
            SublessPayPalId = stripeOptions.Value.SublessPayPalId ?? throw new ArgumentNullException(nameof(stripeOptions));
        }

        public void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null)
        {
            var calculatorResult = _calculatorService.CalculatePayoutsOverRange(startDate, endDate, selectedUserIds);
            if (calculatorResult == null)
            {
                _logger.LogWarning("No Payments found in payment period, distribution skipped.");
                return;
            }

            // rollover idle customers
            foreach (var idleCustomer in calculatorResult.IdleCustomerRollovers)
            {
                _stripeService.RolloverPaymentForIdleCustomer(idleCustomer.CustomerId);
                emailService.SendPatronRolloverReceiptEmail(idleCustomer.CognitoId, idleCustomer.Payment, startDate, endDate);
            }

            // save records for unvisited creators
             
            // send emails
            foreach (var payer in calculatorResult.PaymentsPerPayer)
            {
                _paymentLogsService.SaveLogs(payer.Value);
                emailService.SendReceiptEmail(payer.Value, payer.Key, startDate, endDate);
                calculatorResult.EmailSent = true;
            }
            foreach (var payee in calculatorResult.AllPayouts)
            {
                if (payee.PayeeType == PayeeType.Creator)
                {
                    emailService.SendCreatorReceiptEmail(payee.TargetId, payee, startDate, endDate);
                }
                if (payee.PayeeType == PayeeType.Partner)
                {
                    emailService.SendPartnerReceiptEmail(payee.TargetId, payee, startDate, endDate);
                }
            }
            // record payments to database
            SaveMasterLogs(calculatorResult.AllPayouts);
            // record unvisited to database
            SaveMasterLogs(calculatorResult.UnvisitedCreators);
            // record to s3 bucket
            SavePayoutsToS3(calculatorResult.AllPayouts);
            if (calculatorResult.EmailSent)
            {
                emailService.SendAdminNotification();
            }
        }

        public void SaveFirstPayment()
        {
            _paymentLogsService.SaveLogs(new List<Payment> { new Payment
            {
                Payee = null,
                Payer = null,
                DateSent = DateTime.UtcNow,
                Amount = 0
            }});
        }


        public Guid QueuePayment(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return _calculationQueueRepository.QueuePayment(startDate, endDate);
        }

        public void QueueStripeSync()
        {
            _calculationQueueRepository.QueueStripeSync();
        }

        public void ExecutedQueuedPayment()
        {
            var payment = _calculationQueueRepository.DequeuePayment();
            if (payment != null)
            {
                _logger.LogInformation($"Executing queued payment {payment.Id}");
                ExecutePayments(payment.PeriodStart, payment.PeriodEnd);
                _calculationQueueRepository.CompletePayment(payment);
                _logger.LogInformation($"Completed queued payment {payment.Id}");

            }
        }

        public void QueueIdleEmail(DateTimeOffset start, DateTimeOffset end)
        {
            _calculationQueueRepository.QueueIdleEmails(start, end);
        }

        public void ExecuteQueuedIdleEmail()
        {
            var emails = _calculationQueueRepository.DequeueIdleEmails();
            if (emails != null)
            {
                _logger.LogInformation("Sending idle emails");
                var calculatorResult = _calculatorService.CalculatePayoutsOverRange(emails.PeriodStart, emails.PeriodEnd);
                if (calculatorResult == null) {
                    _logger.LogWarning($"No payments over specified range, skipping sending of idle emails. Start: {emails.PeriodStart} End: {emails.PeriodEnd}");
                    return;
                }

                foreach (var idle in calculatorResult.IdleCustomerRollovers)
                {
                    if (idle.PreviousHits.Any()) {
                        emailService.SendIdleWithHistoryEmail(idle.CognitoId, idle.PreviousHits);
                    } else {
                        emailService.SendIdleEmail(idle.CognitoId);
                    }
                }
                _calculationQueueRepository.CompleteIdleEmails(emails);
            }
        }

        public void ExecuteStripeSync()
        {
            var sync = _calculationQueueRepository.DequeueStripeSync();
            if (sync != null)
            {
                foreach(var id in _userService.GetAllCognitoIds())
                {
                    _stripeService.CachePaymentStatus(id);
                }
                _calculationQueueRepository.CompletsStripeSync(sync);
            }

        }

        private void SaveMasterLogs(IEnumerable<PaymentAuditLog> masterPayoutList)
        {
            _logger.LogInformation("Saving our audit logs.");
            _paymentLogsService.SaveAuditLogs(masterPayoutList);
        }



        private void SavePayoutsToS3(List<PaymentAuditLog> masterPayoutList)
        {
            _logger.LogInformation("Writing out payout information to cloud stoarge.");
            _s3Service.WritePaymentsToCloudFileStore(masterPayoutList);
        }
    }
}
