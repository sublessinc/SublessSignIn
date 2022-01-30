using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class HitService : IHitService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly ICreatorService _creatorService;
        private readonly IPartnerService _partnerService;
        private readonly ILogger _logger;

        public HitService(
            IUserService userService,
            IUserRepository userRepository,
            ICreatorService creatorService,
            IPartnerService partnerService,
            ILoggerFactory loggerFactory
            )
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _logger = loggerFactory?.CreateLogger<HitService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void SaveHit(string userId, Uri uri)
        {
            _logger.LogDebug("SaveHit hit.");
            var partner = _partnerService.GetCachedPartnerByUri(new Uri(uri.GetLeftPart(UriPartial.Authority)));
            if (partner == null)
            {
                _logger.LogError($"Unknown partner recieved hit from URL {uri}");
                return;
            }
            var creatorId = GetCreatorFromPartnerAndUri(uri, partner);
            _logger.LogDebug($"Saving a hit for creator {creatorId}.");
            _userRepository.SaveHit(new Hit()
            {
                CognitoId = userId,
                Uri = uri,
                TimeStamp = DateTimeOffset.UtcNow,
                PartnerId = partner.Id,
                CreatorId = creatorId ?? Guid.Empty
            });
        }

        public Hit TestHit(string userId, Uri uri)
        {
            _logger.LogDebug($"TestHit hit with user {userId} and uri {uri}");
            var partner = _partnerService.GetCachedPartnerByUri(new Uri(uri.GetLeftPart(UriPartial.Authority)));
            _logger.LogDebug($"Resolved partner {partner} for {uri}");
            if (partner == null)
            {
                _logger.LogError($"Unknown partner recieved hit from URL {uri}");
                return null;
            }
            var creatorId = GetCreatorFromPartnerAndUri(uri, partner);
            _logger.LogDebug($"Resolved {creatorId} for the uri {uri} and partner {partner}");

            return new Hit()
            {
                CognitoId = userId,
                Uri = uri,
                TimeStamp = DateTimeOffset.UtcNow,
                PartnerId = partner.Id,
                CreatorId = creatorId ?? Guid.Empty
            };
        }


        public IEnumerable<Hit> GetHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId)
        {
            var user = _userService.GetUser(userId);
            return _userRepository.GetValidHitsByDate(startDate, endDate, user.CognitoId);
        }

        public IEnumerable<Hit> GetCreatorHitsByDate(
            DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId)
        {
            return _userRepository.GetCreatorHitsByDate(startDate, endDate, creatorId);
        }

        public IEnumerable<Hit> GetPartnerHitsByDate(
    DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId)
        {
            return _userRepository.GetPartnerHitsByDate(startDate, endDate, partnerId);
        }

        public Guid? GetCreatorFromPartnerAndUri(Uri uri, Partner partner)
        {
            const string creatorPlaceholder = "{creator}";

            //TODO, what do with this user
            ///www.partner.com/stories

            ///www.partner.com/creator/pictures; www.partner.com/profile/creator; www.partner.com/stories/creator/
            ///www.partner.com\/profile\/?.*\.php
            var patterns = partner.UserPattern.Split(";");
            _logger.LogDebug($"Parter {partner.Site} has {patterns.Count()} patterns to try.");

            // iterate through the possible patterns
            foreach (var pattern in patterns)
            {
                var patternUri = new Uri(pattern);
                if (uri.Segments.Count() < patternUri.Segments.Count())
                {
                    continue;
                }
                var regexPattern = "(?:" + pattern.Replace(creatorPlaceholder, ")([^/]*)(?:") + ")";
                var matches = Regex.Matches(uri.ToString(), regexPattern);
                if (!matches.Any())
                {
                    continue;
                }
                foreach (Match match in matches)
                {
                    _logger.LogDebug($"Matched the uri {uri} using the pattern {pattern}");
                    foreach (Group group in match.Groups)
                    {
                        if (group.Value != uri.ToString() && !string.IsNullOrWhiteSpace(group.Value) && !PartnerService.InvalidUsernameCharacters.Any(x => group.Value.Contains(x)))
                        {
                            return _creatorService.GetCachedCreatorFromPartnerAndUsername(group.Value, partner.Id)?.Id;
                        }
                    }
                }
            }

            _logger.LogDebug($"Couldn't find a creator for the uri {uri} and partner {partner}");
            return null;
        }
    }
}
