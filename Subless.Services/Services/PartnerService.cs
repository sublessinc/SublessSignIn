using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;
using Subless.Services.Extensions;

namespace Subless.Services
{
    public class PartnerService : IPartnerService
    {
        public static List<char> InvalidUsernameCharacters = new List<char> { ';', '/', '?', ':', '&', '=', '+', ',', '$' };

        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache cache;
        private readonly HttpClient httpClient;
        private readonly ILogger<PartnerService> logger;

        public PartnerService(
            IUserRepository userRepository,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILoggerFactory loggerFactory
            )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = loggerFactory.CreateLogger<PartnerService>();
        }

        public Partner GetPartner(Guid id)
        {
            return _userRepository.GetPartner(id);
        }

        public Partner GetPartnerByAdminId(Guid adminId)
        {
            return _userRepository.GetPartnerByAdminId(adminId);
        }

        public Partner UpdatePartnerPayPalId(Guid partnerId, string payPalId)
        {
            var partner = _userRepository.GetPartner(partnerId);
            partner.PayPalId = payPalId;
            _userRepository.UpdatePartner(partner);
            return partner;
        }

        public Partner GetPartnerByCognitoClientId(string cognitoId)
        {
            var partner = _userRepository.GetPartnerByCognitoId(cognitoId);
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
            var creator = _userRepository.GetCreatorByPartnerAndUsername(cognitoClientId, creatorUsername);
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
            creator.ActivationExpiration = DateTime.UtcNow.AddMinutes(10);
            _userRepository.UpsertCreator(creator);
            return code;
        }

        public Guid CreatePartner(Partner partner)
        {
            partner.Site = new Uri(partner.Site.GetLeftPart(UriPartial.Authority));
            _userRepository.AddPartner(partner);
            return partner.Id;
        }


        public void UpdatePartner(Partner partner)
        {
            _userRepository.UpdatePartner(partner);
        }

        public IEnumerable<Partner> GetPartners()
        {
            return _userRepository.GetPartners();
        }

        public Partner GetCachedPartnerByUri(Uri uri)
        {
            if (cache.TryGetValue(uri.ToString(), out Partner partner))
            {
                return (Partner)cache.Get(uri.ToString());
            }
            partner = _userRepository.GetPartnerByUri(uri);
            cache.Set(uri.ToString(), partner, DateTime.UtcNow.AddHours(1));
            return partner;
        }

        public async Task CreatorChangeWebhook(PartnerViewCreator creator)
        {
            this.logger.LogInformation($"Creator {creator.Id} activated, firing webhook");
            var partner = GetPartner(creator.PartnerId);
            if (partner.CreatorWebhook != null) 
            {
                try
                {
                    var viewModel = JsonContent.Create(creator);

                    var result = await httpClient.PostAsync(partner.CreatorWebhook, viewModel);
                    result.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    logger.LogError("Webhook failed \n "+
                        $"URI: {partner.CreatorWebhook} \n" +
                        $"Result: {e.Message} \n" +
                        $"Creator: {creator.Id} \n" +
                        $"Error: {e.Message} \n" +
                        $"Stack: {e.StackTrace} \n" +
                        $"InnerException: {e.InnerException}"
                        );
                }                
            }
        }
    }
}
