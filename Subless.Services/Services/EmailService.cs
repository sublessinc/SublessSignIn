using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<GeneralConfig> _options;
        public ILogger<EmailService> logger;
        public EmailService(ILoggerFactory loggerFactory, IOptions<GeneralConfig> options)
        {
            logger = loggerFactory?.CreateLogger<EmailService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task SendEmail(string body, string to, string subject, string from = "contact@subless.com")
        {
            if (_options.Value.Environment != null && _options.Value.Environment != "prod")
            {
                subject = $"[{_options.Value.Environment}] {subject}";
            }
            using (var client = new AmazonSimpleEmailServiceClient(region: Amazon.RegionEndpoint.USEast2))
            {

                var sendRequest = new SendEmailRequest
                {
                    Source = from,
                    Destination = new Destination
                    {
                        ToAddresses =
                        new List<string> { to }
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
                    logger.LogInformation("Sending email using Amazon SES...");
                    var response = await client.SendEmailAsync(sendRequest);
                    logger.LogInformation("The email was sent successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogCritical("The email was not sent.");
                    logger.LogCritical("Error message: " + ex.Message);

                }
            }
        }
    }
}
