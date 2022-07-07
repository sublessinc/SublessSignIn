using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.PayoutCalculator;

namespace Subless.Services.Services
{
    public class PaymentEmailService : IPaymentEmailService
    {
        public const string MonthKey = "{{month}}"; // March, 2022
        public const string IndividualPaymentTemplate = "            <tr style='padding: 10px;padding-bottom: 20px;'><td>{{creatorname}}</td><td style='text-align: right;'>{{creatorpayment}}</td></tr>";
        public const string PaymentsKey = "{{payments}}"; // list of the above
        public const string CreatorNameKey = "{{creatorname}}"; // CreatorUserName
        public const string CreatorPaymentKey = "{{creatorpayment}}"; //$50.00
        public const string TotalPaymentKey = "{{totalpayment}}"; //$500.00
        public const string RolloverKey = "{{rollover}}"; //$500.00
        public const string StripeFeeKey = "{{stripefee}}"; //$0.10
        public const string SiteLinkKey = "{{sitelink}}"; // https://pay.subless.com
        public const string LogoUrl = "{{logourl}}"; //https://pay.subless.com/SublessLogo.svg

        public const string PayPalFeesKey = "{{paypalfees}}";

        public readonly CalculatorConfiguration authSettings;
        private readonly ICognitoService cognitoService;
        private readonly ICreatorService _creatorService;
        private readonly IPartnerService _partnerService;
        private readonly IUserService _userService;
        private readonly ILogger logger;
        private readonly IEmailService _emailSerivce;

        public PaymentEmailService(
            IOptions<CalculatorConfiguration> options,
            IEmailService emailService,
            ICognitoService cognitoService,
            ICreatorService creatorService,
            IPartnerService partnerService,
            IUserService userService,
            ILoggerFactory loggerFactory)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (options.Value.Domain is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            authSettings = options.Value;
            this.cognitoService = cognitoService ?? throw new ArgumentNullException(nameof(cognitoService));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailSerivce = emailService ?? throw new ArgumentNullException(nameof(emailService));
            logger = loggerFactory.CreateLogger<PaymentEmailService>();
        }

        public void SendReceiptEmail(List<Payment> payments, string cognitoId, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var usertask = Task.Run(() => cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            if (usertask.Result == null)
            {
                logger.LogInformation($"No email present for cognitoid {cognitoId}");
                return;
            }
            var body = GetEmailBody(payments, PaymentPeriodStart, PaymentPeriodEnd);
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Your subless receipt"));
            emailTask.Wait();
        }


        public void SendCreatorReceiptEmail(Guid id, PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {

            var creator = _creatorService.GetCreator(id);
            var body = GetCreatorEmailBody(paymentAuditLog, PaymentPeriodStart, PaymentPeriodEnd);
            var email = GetEmailFromUserId(creator.UserId.Value);
            if (email == null)
            {
                logger.LogInformation($"No email present for user {creator.UserId.Value}");
                return;
            }
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, email, "Your subless payout receipt"));
            emailTask.Wait();
        }

        public void SendPartnerReceiptEmail(Guid id, PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var body = GetPartnerEmailBody(paymentAuditLog, PaymentPeriodStart, PaymentPeriodEnd);
            var partner = _partnerService.GetPartner(id);
            var email = GetEmailFromUserId(partner.Admin);
            if (email == null)
            {
                logger.LogInformation($"No email present for user {partner.Admin}");
                return;
            }
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, email, "Your subless payout receipt"));
            emailTask.Wait();
        }

        public void SendPatronRolloverReceiptEmail(string cognitoId, double payment, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var usertask = Task.Run(() => cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            if (usertask.Result == null)
            {
                logger.LogInformation($"No email present for cognitoid {cognitoId}");
                return;
            }
            var body = GetRolloverEmailBody(payment, PaymentPeriodStart, PaymentPeriodEnd);
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Your subless rollover receipt"));
            emailTask.Wait();
        }

        private string GetEmailFromUserId(Guid userId)
        {
            var user = _userService.GetUser(userId);
            var usertask = Task.Run(() => cognitoService.GetCognitoUserEmail(user.CognitoId));
            usertask.Wait();
            return usertask.Result;
        }

        public void SendAdminNotification()
        {
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(
                $"Receipts have been sent to patrons for { authSettings.Domain }",
                "contact@subless.com",
                $"Receipts have been sent to patrons for { authSettings.Domain }"));
            emailTask.Wait();
        }

        public string GetEmailBody(List<Payment> payments, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var template = GetEmailTemplate();
            return GenerateEmailBodyForUser(template, payments, PaymentPeriodStart, PaymentPeriodEnd);
        }

        private string GetCreatorEmailBody(PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var template = GetCreatorEmailTemplate();
            return GenerateEmailBodyForCreator(template, paymentAuditLog, PaymentPeriodStart, PaymentPeriodEnd);
        }

        private string GetPartnerEmailBody(PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var template = GetPartnerEmailTemplate();
            return GenerateEmailBodyForPartner(template, paymentAuditLog, PaymentPeriodStart, PaymentPeriodEnd);
        }

        private string GetRolloverEmailBody(double payment, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var template = GetRolloverEmailTemplate();
            return GenerateEmailBodyForRolloverPatron(template, payment, PaymentPeriodStart, PaymentPeriodEnd);
        }

        private string GetEmailTemplate()
        {
            var fileName = "Subless.Services.Assets.Receipt.html";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GetCreatorEmailTemplate()
        {
            var fileName = "Subless.Services.Assets.CreatorReceipt.html";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GetPartnerEmailTemplate()
        {
            var fileName = "Subless.Services.Assets.CreatorReceipt.html";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GetRolloverEmailTemplate()
        {
            var fileName = "Subless.Services.Assets.RolloverReceipt.html";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GenerateEmailBodyForUser(string template, List<Payment> payments, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var month = $"{PaymentPeriodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            var fees = payments.First().Payer.Fees / 100;
            var userEmail = template.Replace(MonthKey, month, StringComparison.Ordinal);
            userEmail = userEmail.Replace(SiteLinkKey, authSettings.Domain, StringComparison.Ordinal);
            userEmail = userEmail.Replace(LogoUrl, authSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            userEmail = userEmail.Replace(PaymentsKey, String.Join("\n", GetPaymentItems(payments)), StringComparison.Ordinal);
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var total = payments.Sum(x => x.Amount / 100);
            total = total + fees;
            userEmail = userEmail.Replace(TotalPaymentKey, total.ToString(specifier, culture), StringComparison.Ordinal);
            userEmail = userEmail.Replace(StripeFeeKey, fees.ToString(specifier, culture), StringComparison.Ordinal);
            return userEmail;
        }

        private string GenerateEmailBodyForCreator(string template, PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var month = $"{PaymentPeriodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            var creatorEmail = template.Replace(MonthKey, month, StringComparison.Ordinal);
            creatorEmail = creatorEmail.Replace(SiteLinkKey, authSettings.Domain, StringComparison.Ordinal);
            creatorEmail = creatorEmail.Replace(LogoUrl, authSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            creatorEmail = creatorEmail.Replace(CreatorPaymentKey, paymentAuditLog.Revenue.ToString(specifier, culture), StringComparison.Ordinal);
            creatorEmail = creatorEmail.Replace(TotalPaymentKey, paymentAuditLog.Payment.ToString(specifier, culture), StringComparison.Ordinal);
            creatorEmail = creatorEmail.Replace(PayPalFeesKey, paymentAuditLog.Fees.ToString(specifier, culture), StringComparison.Ordinal);
            return creatorEmail;
        }

        private string GenerateEmailBodyForPartner(string template, PaymentAuditLog paymentAuditLog, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var month = $"{PaymentPeriodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            var partnerEmail = template.Replace(MonthKey, month, StringComparison.Ordinal);
            partnerEmail = partnerEmail.Replace(SiteLinkKey, authSettings.Domain, StringComparison.Ordinal);
            partnerEmail = partnerEmail.Replace(LogoUrl, authSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            partnerEmail = partnerEmail.Replace(CreatorPaymentKey, paymentAuditLog.Revenue.ToString(specifier, culture), StringComparison.Ordinal);
            partnerEmail = partnerEmail.Replace(TotalPaymentKey, paymentAuditLog.Payment.ToString(specifier, culture), StringComparison.Ordinal);
            partnerEmail = partnerEmail.Replace(PayPalFeesKey, paymentAuditLog.Fees.ToString(specifier, culture), StringComparison.Ordinal);
            return partnerEmail;
        }

        private string GenerateEmailBodyForRolloverPatron(string template, double payment, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var month = $"{PaymentPeriodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            var patronEmail = template.Replace(MonthKey, month, StringComparison.Ordinal);
            patronEmail = patronEmail.Replace(SiteLinkKey, authSettings.Domain, StringComparison.Ordinal);
            patronEmail = patronEmail.Replace(LogoUrl, authSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            patronEmail = patronEmail.Replace(TotalPaymentKey, payment.ToString(specifier, culture), StringComparison.Ordinal);
            patronEmail = patronEmail.Replace(RolloverKey, payment.ToString(specifier, culture), StringComparison.Ordinal);
            return patronEmail;
        }

        private List<string> GetPaymentItems(List<Payment> payments)
        {
            List<string> formattedPayments = new List<string>();
            foreach (Payment payment in payments)
            {
                var individualPayment = IndividualPaymentTemplate.Replace(CreatorNameKey, payment.Payee.Name, StringComparison.Ordinal);
                var specifier = "C";
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                individualPayment = individualPayment.Replace(CreatorPaymentKey, (payment.Amount / 100).ToString(specifier, culture), StringComparison.Ordinal);
                formattedPayments.Add(individualPayment);
            }
            return formattedPayments;
        }
    }
}
