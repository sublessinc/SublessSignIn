using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;

namespace Subless.Services.Services
{
    public class PartnerService : IPartnerService
    {
        public static List<char> InvalidUsernameCharacters = new List<char> { ';', '/', '?', ':', '&', '=', '+', ',', '$' };

        private readonly IUserRepository _userRepository;
        private readonly IPartnerRepository partnerRepository;
        private readonly ICreatorRepository creatorRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICacheService cache;
        private readonly HttpClient httpClient;
        private readonly ILogger<PartnerService> logger;

        public PartnerService(
            IUserRepository userRepository,
            IPartnerRepository partnerRepository,
            ICreatorRepository creatorRepository,
            IHttpClientFactory httpClientFactory,
            IPaymentRepository paymentRepository,
            ICacheService cache,
            ILoggerFactory loggerFactory
            )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.partnerRepository = partnerRepository ?? throw new ArgumentNullException(nameof(partnerRepository));
            this.creatorRepository = creatorRepository ?? throw new ArgumentNullException(nameof(creatorRepository));
            _paymentRepository = paymentRepository;
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            logger = loggerFactory.CreateLogger<PartnerService>();
        }

        public Partner GetPartner(Guid id)
        {
            return partnerRepository.GetPartner(id);
        }

        public Partner GetPartnerByAdminId(Guid adminId)
        {
            return partnerRepository.GetPartnerByAdminId(adminId);
        }

        public Partner UpdatePartnerWritableFields(PartnerWriteModel partnerModel)
        {
            var partner = partnerRepository.GetPartner(partnerModel.Id);
            partner.PayPalId = partnerModel.PayPalId;
            partner.CreatorWebhook = partnerModel.CreatorWebhook;
            partner.Favicon = partnerModel.Favicon;
            partnerRepository.UpdatePartner(partner);
            return partner;
        }

        public Partner GetPartnerByCognitoClientId(string cognitoId)
        {
            var partner = partnerRepository.GetPartnerByAppClientId(cognitoId);
            return partner;
        }

        public Guid GenerateCreatorActivationLink(string cognitoClientId, string creatorUsername)
        {

            var partner = GetPartnerByCognitoClientId(cognitoClientId);
            if (partner == null)
            {
                throw new UnauthorizedAccessException("Partner not found. Partner may not have been activated yet.");
            }
            var partnerId = partner.Id;
            var creator = partnerRepository.GetCreatorByPartnerAndUsername(cognitoClientId, creatorUsername);
            if (creator == null)
            {
                creator = new Creator()
                {
                    PartnerId = partnerId,
                    Active = false,
                    Username = creatorUsername
                };
            }
            if (creator.Active)
            {
                throw new CreatorAlreadyActiveException();
            }
            var code = Guid.NewGuid();
            creator.ActivationCode = code;
            creator.ActivationExpiration = DateTimeOffset.UtcNow.AddMinutes(10);
            creatorRepository.UpsertCreator(creator);
            cache.InvalidateCache();
            return code;
        }

        public Guid CreatePartner(Partner partner)
        {
            partner.Sites = partner.Sites.Select(x => new Uri(x.GetLeftPart(UriPartial.Authority))).ToArray();
            partnerRepository.AddPartner(partner);
            cache.InvalidateCache();
            return partner.Id;
        }


        public void UpdatePartner(Partner partner)
        {
            partnerRepository.UpdatePartner(partner);
            cache.InvalidateCache();
        }

        public IEnumerable<Partner> GetPartners()
        {
            return partnerRepository.GetPartners();
        }

        public Partner GetCachedPartnerByUri(Uri uri)
        {
            if (cache.Cache.TryGetValue(uri.ToString(), out Partner _))
            {
                return (Partner)cache.Cache.Get(uri.ToString());
            }
            var partner = partnerRepository.GetPartnerByUri(uri);
            cache.Cache.Set(uri.ToString(), partner, DateTimeOffset.UtcNow.AddMinutes(15));
            return partner;
        }

        public IEnumerable<MonthlyPaymentStats> GetStatsForPartner(Partner partner)
        {
            if (partner is null)
            {
                throw new ArgumentNullException(nameof(partner));
            }
            var paymentStats = new List<MonthlyPaymentStats>();
            var paymentAuditLogs = _paymentRepository.GetAllPaymentsToUser(partner.Id);
            foreach (var payment in paymentAuditLogs)
            {
                paymentStats.Add(new MonthlyPaymentStats()
                {
                    MonthStart = payment.PaymentPeriodStart,
                    Revenue = payment.Revenue,
                    PaymentProcessorFees = payment.Fees,
                    Payment = payment.Payment,
                    MonthEnd = payment.PaymentPeriodEnd
                });

            }
            return paymentStats.OrderBy(x => x.MonthStart);
        }


        public async Task<bool> CreatorChangeWebhook(PartnerViewCreator creator)
        {
            logger.LogInformation($"Creator {creator.Id} activated, firing webhook");
            var partner = GetPartner(creator.PartnerId);
            if (partner.CreatorWebhook != null)
            {
                try
                {
                    var viewModel = JsonContent.Create(creator);

                    var result = await httpClient.PostAsync(partner.CreatorWebhook, viewModel);
                    result.EnsureSuccessStatusCode();
                    return true;
                }
                catch (Exception e)
                {
                    logger.LogError("Webhook failed \n " +
                        $"URI: {partner.CreatorWebhook} \n" +
                        $"Result: {e.Message} \n" +
                        $"Creator: {creator.Id} \n" +
                        $"Error: {e.Message} \n" +
                        $"Stack: {e.StackTrace} \n" +
                        $"InnerException: {e.InnerException}"
                        );
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<string> GetParterUris()
        {
            return partnerRepository.GetPartnerUris().Select(x => x.ToString());
        }
        public void AcceptTerms(string cognitoId)
        {
            var partners = _userRepository.GetPartnersByCognitoId(cognitoId);
            if (partners != null && partners.Any())
            {
                var partner = partners.First();
                partner.AcceptedTerms = true;
                partnerRepository.UpdatePartner(partner);
            }
        }
    }
}

