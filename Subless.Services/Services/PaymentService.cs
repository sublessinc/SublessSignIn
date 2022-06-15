using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IPaymentEmailService emailService;
        private readonly ICalculatorService _calculatorService;
        private readonly ICreatorService _creatorService;
        private readonly ILogger _logger;

        public PaymentService(
            IStripeService stripeService,
            IPaymentLogsService paymentLogsService,
            IFileStorageService s3Service,
            IOptions<StripeConfig> stripeOptions,
            IPaymentEmailService emailService,
            ICalculatorService calculatorService,
            ICreatorService creatorService,
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
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _logger = _loggerFactory.CreateLogger<PaymentService>();
            SublessPayPalId = stripeOptions.Value.SublessPayPalId ?? throw new ArgumentNullException(nameof(stripeOptions));
        }

        public void ExecutePayments(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null)
        {
            var calculatorResult = _calculatorService.CaculatePayoutsOverRange(startDate, endDate, selectedUserIds);
            if (calculatorResult == null)
            {
                _logger.LogWarning("No Payments found in payment period, distribution skipped.");
                return;
            }

            // rollover idle customers
            foreach (var idleCustomerId in calculatorResult.IdleCustomerStripeIds)
            {
                _stripeService.RolloverPaymentForIdleCustomer(idleCustomerId);
            }
            // send emails
            foreach (var payer in calculatorResult.PaymentsPerPayer)
            {
                _paymentLogsService.SaveLogs(payer.Value);
                emailService.SendReceiptEmail(payer.Value, payer.Key);
                calculatorResult.EmailSent = true;
            }
            foreach (var payee in calculatorResult.AllPayouts)
            {
                if (payee.PayeeType == PayeeType.Creator)
                {
                    var creator = _creatorService.GetCreator(payee.TargetId);
                    emailService.SendCreatorReceiptEmail(creator.Email, payee);
                }
                if (payee.PayeeType == PayeeType.Partner)
                {
                    // TODO: we don't save a partner email anywhere yet, so I'm using the paypal address.
                    // This will be a problem if the paypal address is a phone number
                    emailService.SendPartnerReceiptEmail(payee.PayPalId, payee);
                }
                
            }
            // record to database
            SaveMasterList(calculatorResult.AllPayouts);
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

        private void SaveMasterList(List<PaymentAuditLog> masterPayoutList)
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
