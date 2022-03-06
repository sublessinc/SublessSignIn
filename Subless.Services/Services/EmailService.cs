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

namespace Subless.Services.Services
{
    public class EmailService : IEmailService
    {
        public const string MonthKey = "{{month}}"; // March, 2022
        public const string IndividualPaymentTemplate = "            <div class='item'><div>{{creatorname}}</div><div>{{creatorpayment}}</div></div>";
        public const string PaymentsKey = "{{payments}}"; // list of the above
        public const string CreatorNameKey = "{{creatorname}}"; // CreatorUserName
        public const string CreatorPaymentKey = "{{creatorpayment}}"; //$50.00
        public const string TotalPaymentKey = "{{totalpayment}}"; //$500.00
        public const string SiteLinkKey = "{{sitelink}}"; // https://pay.subless.com
        public readonly AuthSettings authSettings;
        public EmailService(IOptions<AuthSettings> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            authSettings = options.Value;
        }

        public string GetEmailBody(List<Payment> payments)
        {
            var template = GetEmailTemplate();
            return GenerateEmailBodyForUser(template, payments);
        }

        private string GetEmailTemplate()
        {
            var fileName = "Receipt.html";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GenerateEmailBodyForUser(string template, List<Payment> payments)
        {
            var month = $"{payments.First().DateSent.ToString("MMMM")}, {payments.First().DateSent.ToString("yyyy")}";
            var userEmail = template.Replace(MonthKey, month);
            userEmail = userEmail.Replace(SiteLinkKey, authSettings.Domain);
            userEmail.Replace(PaymentsKey, String.Join("\n", GetPaymentItems(payments)));
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            userEmail.Replace(TotalPaymentKey, payments.Sum(x => x.Amount).ToString(specifier, culture));
            return userEmail;
        }

        private List<string> GetPaymentItems(List<Payment> payments)
        {
            List<string> formattedPayments = new List<string>();
            foreach (Payment payment in payments)
            {
                var individualPayment = IndividualPaymentTemplate.Replace(CreatorNameKey, payment.Payee.Name);
                var specifier = "C";
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                individualPayment = individualPayment.Replace(CreatorPaymentKey, payment.Payee.Payment.ToString(specifier, culture));
                formattedPayments.Add(individualPayment);
            }
            return formattedPayments;
        }
    }
}
