using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Reflection;
using System.IO;
using Subless.Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using Amazon.SimpleEmail;
using Amazon;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;

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
        public const string StripeFeeKey = "{{stripefee}}"; //$0.10
        public const string SiteLinkKey = "{{sitelink}}"; // https://pay.subless.com
        public const string LogoUrl = "{{logourl}}"; //https://pay.subless.com/SublessLogo.svg
        public readonly CalculatorConfiguration authSettings;
        private readonly ICognitoService cognitoService;
        private readonly ILogger logger;
        private readonly IEmailService _emailSerivce;

        public PaymentEmailService(IOptions<CalculatorConfiguration> options, IEmailService emailService, ICognitoService cognitoService, ILoggerFactory loggerFactory)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value.Domain is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            authSettings = options.Value;
            this.cognitoService = cognitoService ?? throw new ArgumentNullException(nameof(cognitoService));
            _emailSerivce= emailService?? throw new ArgumentNullException(nameof(emailService));
            logger = loggerFactory.CreateLogger<PaymentEmailService>();
        }

        public void SendReceiptEmail(List<Payment> payments, string cognitoId)
        {
            var usertask = Task.Run(() => cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            var body = GetEmailBody(payments);
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Your subless receipt"));
            emailTask.Wait();
        }

        public void SendAdminNotification()
        {
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(
                $"Receipts have been sent to patrons for { authSettings.Domain }",
                "contact@subless.com",
                $"Receipts have been sent to patrons for { authSettings.Domain }"));
            emailTask.Wait();
        }

        public string GetEmailBody(List<Payment> payments)
        {
            var template = GetEmailTemplate();
            return GenerateEmailBodyForUser(template, payments);
        }

        private string GetEmailTemplate()
        {
            var fileName = "Subless.PayoutCalculator.Assets.Receipt.html";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GenerateEmailBodyForUser(string template, List<Payment> payments)
        {
            var month = $"{payments.First().DateSent.ToString("MMMM", CultureInfo.InvariantCulture)}, {payments.First().DateSent.ToString("yyyy", CultureInfo.InvariantCulture)}";
            var fees = payments.First().Payer.Fees/100;
            var userEmail = template.Replace(MonthKey, month, StringComparison.Ordinal);
            userEmail = userEmail.Replace(SiteLinkKey, authSettings.Domain, StringComparison.Ordinal);
            userEmail = userEmail.Replace(LogoUrl, authSettings.Domain+ "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            userEmail = userEmail.Replace(PaymentsKey, String.Join("\n", GetPaymentItems(payments)), StringComparison.Ordinal);
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var total = payments.Sum(x => x.Amount / 100);
            total = total + fees;
            userEmail = userEmail.Replace(TotalPaymentKey, total.ToString(specifier, culture), StringComparison.Ordinal);
            userEmail = userEmail.Replace(StripeFeeKey, fees.ToString(specifier, culture), StringComparison.Ordinal);
            return userEmail;
        }

        private List<string> GetPaymentItems(List<Payment> payments)
        {
            List<string> formattedPayments = new List<string>();
            foreach (Payment payment in payments)
            {
                var individualPayment = IndividualPaymentTemplate.Replace(CreatorNameKey, payment.Payee.Name, StringComparison.Ordinal);
                var specifier = "C";
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                individualPayment = individualPayment.Replace(CreatorPaymentKey, (payment.Amount/100).ToString(specifier, culture), StringComparison.Ordinal);
                formattedPayments.Add(individualPayment);
            }
            return formattedPayments;
        }
    }
}
