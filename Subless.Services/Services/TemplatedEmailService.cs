﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.PayoutCalculator;

namespace Subless.Services.Services
{
    public class TemplatedEmailService : ITemplatedEmailService
    {
        public const string MonthKey = "{{month}}"; // March, 2022
        public const string IndividualPaymentTemplate = "<tr style='padding: 10px;padding-bottom: 20px; border-bottom: solid 1px #f2f2f2;'>                <td>{{creatorname}}</td>                <td>{{creatorMessage}}</td>                <td style='text-align: right;'>{{creatorpayment}}</td>            </tr>";
        public const string PaymentsKey = "{{payments}}"; // list of the above
        public const string CreatorNameKey = "{{creatorname}}"; // CreatorUserName
        public const string CreatorPaymentKey = "{{creatorpayment}}"; //$50.00
        public const string TotalPaymentKey = "{{totalpayment}}"; //$500.00
        public const string RolloverKey = "{{rollover}}"; //$500.00
        public const string StripeFeeKey = "{{stripefee}}"; //$0.10
        public const string SiteLinkKey = "{{sitelink}}"; // https://pay.subless.com
        public const string LogoUrl = "{{logourl}}"; //https://pay.subless.com/SublessLogo.svg
        public const string CreatorMessageKey = "{{creatorMessage}}";
        public const string PayPalFeesKey = "{{paypalfees}}";
        public const string IdleEmailHistoryListKey = "{{idleEmailHistoryList}}";

        public readonly CalculatorConfiguration _calculatorConfigurationAuthSettings;
        private readonly IOptions<AuthSettings> _authOptions;
        private readonly ICognitoService _cognitoService;
        private readonly ICreatorService _creatorService;
        private readonly IPartnerService _partnerService;
        private readonly IUserService _userService;
        private readonly ILogger logger;
        private readonly IEmailService _emailSerivce;

        public TemplatedEmailService(
            IOptions<CalculatorConfiguration> calculatorConfigurationOptions,
            IOptions<AuthSettings> authOptions,
            IEmailService emailService,
            ICognitoService cognitoService,
            ICreatorService creatorService,
            IPartnerService partnerService,
            IUserService userService,
            ILoggerFactory loggerFactory)
        {
            if (calculatorConfigurationOptions is null)
            {
                throw new ArgumentNullException(nameof(calculatorConfigurationOptions));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (calculatorConfigurationOptions.Value.Domain is null)
            {
                throw new ArgumentNullException(nameof(calculatorConfigurationOptions));
            }
            
            _calculatorConfigurationAuthSettings = calculatorConfigurationOptions.Value;
            _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
            if (string.IsNullOrWhiteSpace(_authOptions.Value.Domain)) {
                throw new ArgumentNullException(nameof(authOptions.Value.Domain));
            }
            
            _cognitoService = cognitoService ?? throw new ArgumentNullException(nameof(cognitoService));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailSerivce = emailService ?? throw new ArgumentNullException(nameof(emailService));
            logger = loggerFactory.CreateLogger<TemplatedEmailService>();
        }

        public void SendReceiptEmail(List<Payment> payments, string cognitoId, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            var usertask = Task.Run(() => _cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            if (usertask.Result == null)
            {
                logger.LogInformation($"No email present for cognitoid {cognitoId}");
                return;
            }
            var body = GetEmailBody(payments, PaymentPeriodStart, PaymentPeriodEnd);
            if (new Random().NextDouble() > .5) // A/B testing the subject line
            {
                var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Your subless receipt"));
                emailTask.Wait();
            }
            else
            {
                var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Here's who you supported with subless this month!"));
                emailTask.Wait();
            }
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
            var usertask = Task.Run(() => _cognitoService.GetCognitoUserEmail(cognitoId));
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

        public void SendWelcomeEmail(string cognitoId)
        {
            var usertask = Task.Run(() => _cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            if (usertask.Result == null)
            {
                logger.LogInformation($"No email present for cognitoid {cognitoId}");
                return;
            }
            var body = GetWelcomeEmail();
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Welcome to subless!"));
            emailTask.Wait();
            _userService.WelcomeSent(cognitoId);
        }

        public void SendIdleEmail(string cognitoId)
        {
            var usertask = Task.Run(() => _cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            if (usertask.Result == null)
            {
                logger.LogInformation($"No email present for cognitoid {cognitoId}");
                return;
            }
            var body = GenerateEmailBodyForIdleEmail(GetIdleEmail());
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "Your subless budget isn't going to any creators this month!"));
            emailTask.Wait();
        }
        
        public void SendIdleWithHistoryEmail(string cognitoId, IEnumerable<Hit> previousHits)
        {
            var usertask = Task.Run(() => _cognitoService.GetCognitoUserEmail(cognitoId));
            usertask.Wait();
            if (usertask.Result == null)
            {
                logger.LogInformation($"No email present for cognitoid {cognitoId}");
                return;
            }
            var body = GenerateEmailBodyForIdleWithHistoryEmail(this.GetIdleWithHistoryEmail(), previousHits);
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(body, usertask.Result, "There's one week left to support creators this month! Here's a few you've enjoyed"));
            emailTask.Wait();
        }

        private string GetEmailFromUserId(Guid userId)
        {
            var user = _userService.GetUser(userId);
            var usertask = Task.Run(() => _cognitoService.GetCognitoUserEmail(user.CognitoId));
            usertask.Wait();
            return usertask.Result;
        }

        public void SendAdminNotification()
        {
            var emailTask = Task.Run(() => _emailSerivce.SendEmail(
                $"Receipts have been sent to patrons for { _calculatorConfigurationAuthSettings.Domain }",
                "contact@subless.com",
                $"Receipts have been sent to patrons for { _calculatorConfigurationAuthSettings.Domain }"));
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

        private string GetWelcomeEmail()
        {
            return ReadTemplate("Subless.Services.Assets.Welcome.html");
        }

        public string GetIdleEmail()
        {
            return ReadTemplate("Subless.Services.Assets.Idle.html");
        }

        public string GetIdleWithHistoryEmail()
        {
            return ReadTemplate("Subless.Services.Assets.IdleWithHistory.html");
        }

        private string GetEmailTemplate()
        {
            return ReadTemplate("Subless.Services.Assets.Receipt.html");
        }

        private string GetCreatorEmailTemplate()
        {
            return ReadTemplate("Subless.Services.Assets.CreatorReceipt.html");
        }

        private string GetPartnerEmailTemplate()
        {
            return ReadTemplate("Subless.Services.Assets.CreatorReceipt.html");
        }

        private string GetRolloverEmailTemplate()
        {
            return ReadTemplate("Subless.Services.Assets.RolloverReceipt.html");
        }

        private string ReadTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(templateName);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private string GenerateEmailBodyForIdleEmail(string template)
        {
            var email = template.Replace(SiteLinkKey, _calculatorConfigurationAuthSettings.Domain, StringComparison.Ordinal);
            return email.Replace(LogoUrl, _calculatorConfigurationAuthSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
        }
        
        private string GenerateEmailBodyForIdleWithHistoryEmail(string template, IEnumerable<Hit> previousHits)
        {
            var email = template.Replace(SiteLinkKey, _calculatorConfigurationAuthSettings.Domain, StringComparison.Ordinal);
            email.Replace(LogoUrl, _calculatorConfigurationAuthSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);

            // Select one hit for each creatorId ordered by the number of hits for the creatorId descending
            var orderedHits = previousHits.GroupBy(g => g.CreatorId).OrderByDescending(g => g.Count()).Select(g => g.First()).Take(5);
            var builder = new StringBuilder();
            foreach (var hit in orderedHits) {
                builder.AppendLine(BuildCreatorListItem(hit));
            }

            email = email.Replace(IdleEmailHistoryListKey, builder.ToString());
            return email;
        }

        private string BuildCreatorListItem(Hit hit) {
            var partner = _partnerService.GetPartner(hit.PartnerId);
            var creator = _creatorService.GetCreator(hit.CreatorId);
            var creatorUri = partner.UserPattern.Split(";").First().Replace(Constants.CreatorPlaceholderKey, creator.Username);
            var uriWithLoginRedirect = new Uri(new Uri(_authOptions.Value.Domain, UriKind.Absolute), new Uri($"/bff/login?returnUrl={creatorUri}", UriKind.Relative));

            return $"<li>"
                + $"<a href={uriWithLoginRedirect} style=\"text-decoration:none;\">"
                + $"<img src=\"{partner.Favicon}\" alt=\"\" width=14 height=14 style=\"vertical-align: middle;\">"
                + $"<span style=\"vertical-align:middle;margin-left:4px;\">{creator.Username}</span>"
                + $"</a>"
                + $"</li>";
        }

        private string GenerateEmailBodyForUser(string template, List<Payment> payments, DateTimeOffset PaymentPeriodStart, DateTimeOffset PaymentPeriodEnd)
        {
            
            var month = $"{PaymentPeriodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            var fees = payments.First().Payer.Fees / 100;
            var userEmail = template.Replace(MonthKey, month, StringComparison.Ordinal);
            userEmail = userEmail.Replace(SiteLinkKey, _calculatorConfigurationAuthSettings.Domain, StringComparison.Ordinal);
            userEmail = userEmail.Replace(LogoUrl, _calculatorConfigurationAuthSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            userEmail = userEmail.Replace(PaymentsKey, string.Join("\n", GetPaymentItems(payments)), StringComparison.Ordinal);
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
            creatorEmail = creatorEmail.Replace(SiteLinkKey, _calculatorConfigurationAuthSettings.Domain, StringComparison.Ordinal);
            creatorEmail = creatorEmail.Replace(LogoUrl, _calculatorConfigurationAuthSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
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
            partnerEmail = partnerEmail.Replace(SiteLinkKey, _calculatorConfigurationAuthSettings.Domain, StringComparison.Ordinal);
            partnerEmail = partnerEmail.Replace(LogoUrl, _calculatorConfigurationAuthSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
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
            patronEmail = patronEmail.Replace(SiteLinkKey, _calculatorConfigurationAuthSettings.Domain, StringComparison.Ordinal);
            patronEmail = patronEmail.Replace(LogoUrl, _calculatorConfigurationAuthSettings.Domain + "/dist/assets/SublessLogo.png", StringComparison.Ordinal);
            var specifier = "C";
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            patronEmail = patronEmail.Replace(TotalPaymentKey, payment.ToString(specifier, culture), StringComparison.Ordinal);
            patronEmail = patronEmail.Replace(RolloverKey, payment.ToString(specifier, culture), StringComparison.Ordinal);
            return patronEmail;
        }

        private List<string> GetPaymentItems(List<Payment> payments)
        {

            var formattedPayments = new List<string>();
            foreach (var payment in payments)
            {
                var individualPayment = IndividualPaymentTemplate.Replace(CreatorNameKey, payment.Payee.Name, StringComparison.Ordinal);

                if (payment.Payee.PayeeType == PayeeType.Creator)
                { 
                    var message = _creatorService.GetCreatorMessage(payment.Payee.TargetId);
                    if (message != null && !string.IsNullOrWhiteSpace(message.Message))
                    {
                        individualPayment = individualPayment.Replace(CreatorMessageKey, message.Message, StringComparison.Ordinal);
                    }
                    else
                    {
                        individualPayment = individualPayment.Replace(CreatorMessageKey, String.Empty, StringComparison.Ordinal);
                    }
                }
                else
                {
                    individualPayment = individualPayment.Replace(CreatorMessageKey, String.Empty, StringComparison.Ordinal);
                }

                var specifier = "C";
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                individualPayment = individualPayment.Replace(CreatorPaymentKey, (payment.Amount / 100).ToString(specifier, culture), StringComparison.Ordinal);
                formattedPayments.Add(individualPayment);
            }
            return formattedPayments;
        }
    }
}
