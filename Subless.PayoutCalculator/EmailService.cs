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
using Subless.PayoutCalculator;
using Amazon.SimpleEmail;
using Amazon;
using Amazon.SimpleEmail.Model;

namespace Subless.Services.Services
{
    public class EmailService : IEmailService
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

        public EmailService(IOptions<CalculatorConfiguration> options, ICognitoService cognitoService)
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
        }

        public void SendReceiptEmail(List<Payment> payments, string cognitoId)
        {
            var usertask = Task.Run(() => cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            var body = GetEmailBody(payments);
            var emailTask = Task.Run(() => SendEmail(body, usertask.Result, "contact@subless.com", "Your subless receipt"));
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
            var month = $"{payments.First().DateSent.ToString("MMMM")}, {payments.First().DateSent.ToString("yyyy")}";
            var fees = payments.First().Payer.Fees/100;
            var userEmail = template.Replace(MonthKey, month);
            userEmail = userEmail.Replace(SiteLinkKey, authSettings.Domain);
            userEmail = userEmail.Replace(LogoUrl, authSettings.Domain+ "/dist/assets/SublessLogo.png");
            userEmail = userEmail.Replace(PaymentsKey, String.Join("\n", GetPaymentItems(payments)));
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var total = payments.Sum(x => x.Amount / 100);
            total = total + fees;
            userEmail = userEmail.Replace(TotalPaymentKey, total.ToString(specifier, culture));
            userEmail = userEmail.Replace(StripeFeeKey, fees.ToString(specifier, culture));
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
                individualPayment = individualPayment.Replace(CreatorPaymentKey, (payment.Amount/100).ToString(specifier, culture));
                formattedPayments.Add(individualPayment);
            }
            return formattedPayments;
        }

        public async Task SendEmail(string body, string to, string from, string subject)
        {
            using (var client = new AmazonSimpleEmailServiceClient())
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = to,
                    Destination = new Destination
                    {
                        ToAddresses =
                        new List<string> { from }
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = body
                            },
                        }
                    },
                    // If you are not using a configuration set, comment
                    // or remove the following line 
                    //ConfigurationSetName = configSet
                };
                try
                {
                    Console.WriteLine("Sending email using Amazon SES...");
                    var response = await client.SendEmailAsync(sendRequest);
                    Console.WriteLine("The email was sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);

                }
            }
        }
    }
}
