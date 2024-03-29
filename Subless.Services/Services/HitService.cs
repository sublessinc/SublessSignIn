﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Data;
using Subless.Models;

namespace Subless.Services.Services
{
    public class HitService : IHitService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IHitRepository hitRepository;
        private readonly ICreatorService _creatorService;
        private readonly IPartnerService _partnerService;
        private readonly FeatureConfig _featureConfig;
        private readonly ILogger _logger;

        public HitService(
            IUserService userService,
            IUserRepository userRepository,
            IHitRepository hitRepository,
            ICreatorService creatorService,
            IPartnerService partnerService,
            IOptions<FeatureConfig> featureConfig,
            ILoggerFactory loggerFactory
            )
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _featureConfig = featureConfig?.Value ?? throw new ArgumentNullException(nameof(featureConfig));
            _logger = loggerFactory?.CreateLogger<HitService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public bool SaveHit(string userId, Uri uri)
        {
            _logger.LogDebug("SaveHit hit.");
            var partner = _partnerService.GetCachedPartnerByUri(new Uri(uri.GetLeftPart(UriPartial.Authority)));
            if (partner == null)
            {
                _logger.LogError($"Unknown partner received hit from URL {uri}");
                return false;
            }
            var creatorId = GetCreatorFromPartnerAndUri(uri, partner);
            var hit = new Hit()
            {
                CognitoId = userId,
                Uri = uri,
                TimeStamp = DateTimeOffset.UtcNow,
                PartnerId = partner.Id,
                CreatorId = creatorId ?? Guid.Empty
            };
            _logger.LogDebug($"Saving a hit for creator {creatorId} at time {hit.TimeStamp}.");
            hitRepository.SaveHit(hit);
            if (_featureConfig.HitPopupEnabled)
            {
                return creatorId != null;
            }
            return false;
        }

        public bool SaveTagHit(string userId, Uri uri, string creatorname)
        {
            _logger.LogDebug("Save tag hit.");
            var partner = _partnerService.GetCachedPartnerByUri(new Uri(uri.GetLeftPart(UriPartial.Authority)));
            if (partner == null)
            {
                _logger.LogError($"Unknown partner received hit from URL {uri}");
                return false;
            }
            var creator = _creatorService.GetCachedCreatorFromPartnerAndUsername(creatorname, partner.Id);
            var hit = new Hit()
            {
                CognitoId = userId,
                Uri = uri,
                TimeStamp = DateTimeOffset.UtcNow,
                PartnerId = partner.Id,
                CreatorId = creator?.Id ?? Guid.Empty
            };
            _logger.LogDebug($"Saving a hit for creator {creatorname} id:{creator?.Id} at time {hit.TimeStamp}.");
            hitRepository.SaveHit(hit);
            if (_featureConfig.HitPopupEnabled)
            {
                return creator?.Id != null;
            }
            return false;
        }

        public Hit TestHit(string userId, Uri uri)
        {
            _logger.LogDebug($"TestHit hit with user {userId} and uri {uri}");
            var partner = _partnerService.GetCachedPartnerByUri(new Uri(uri.GetLeftPart(UriPartial.Authority)));
            _logger.LogDebug($"Resolved partner {partner} for {uri}");
            if (partner == null)
            {
                _logger.LogError($"Unknown partner received hit from URL {uri}");
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
            var user = _userService.GetUserWithRelationships(userId);
            _logger.LogDebug($"Getting hits for range {startDate} to {endDate}");
            var hits = hitRepository.GetValidHitsByDate(startDate, endDate, user.CognitoId, user.Creators.FirstOrDefault()?.Id).ToList();
            var creators = _creatorService.FilterInactiveCreators(hits.Select(x => x.CreatorId));
            return hits.Where(x => creators.Contains(x.CreatorId));
        }

        public UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId)
        {
            var user = _userService.GetUserWithRelationships(userId);
            _logger.LogDebug($"Getting hits for range {startDate} to {endDate}");
            return hitRepository.GetUserStats(startDate, endDate, user.CognitoId, user.Creators.FirstOrDefault()?.Id);
        }

        public CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, IEnumerable<Guid> creatorIds, string cognitoId)
        {
            _logger.LogDebug($"Getting hits for range {startDate} to {endDate}");
            return hitRepository.GetCreatorStats(startDate, endDate, creatorIds, cognitoId);
        }

        public PartnerStats GetPartnerStats(
    DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId)
        {
            _logger.LogDebug($"Getting hits for range {startDate} to {endDate}");
            return hitRepository.GetPartnerStats(startDate, endDate, partnerId, cognitoId);
        }

        public IEnumerable<Hit> GetPartnerHitsByDate(
    DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId)
        {
            _logger.LogDebug($"Getting hits for range {startDate} to {endDate}");
            return hitRepository.GetPartnerHitsByDate(startDate, endDate, partnerId, cognitoId);
        }

        public IEnumerable<Hit> FilterOutCreator(IEnumerable<Hit> hits, Guid creatorId)
        {
            return hits.Where(x => x.CreatorId != creatorId);
        }


        public IEnumerable<HitView> GetRecentCreatorContent(Guid creatorId, string cognitoId)
        {
            return hitRepository.GetRecentCreatorContent(creatorId, cognitoId);
        }

        public IEnumerable<ContentHitCount> GetTopCreatorContent(Guid creatorId, string cognitoId)
        {
            return hitRepository.GetTopCreatorContent(creatorId, cognitoId);
        }

        public IEnumerable<HitView> GetRecentPatronContent(string cognitoId)
        {
            var creators = _creatorService.GetCreatorOrDefaultByCognitoid(cognitoId);
            if (creators != null)
            {
                foreach (var creator in creators)
                {
                    return hitRepository.GetRecentPatronContent(cognitoId, creator.Id);
                }
            }
            else
            {
                return hitRepository.GetRecentPatronContent(cognitoId);

            }
            return new List<HitView>();
        }

        public IEnumerable<CreatorHitCount> GetTopPatronContent(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId)
        {
            var creators = _creatorService.GetCreatorOrDefaultByCognitoid(cognitoId);
            if (creators != null) {
                foreach (var creator in creators)
                {
                    var hits = hitRepository.GetTopPatronContent(startDate, endDate, cognitoId, creator.Id);
                    return hits.Select(x =>
                    {
                        x.CreatorName = _creatorService.GetCreator(x.CreatorId)?.Username ?? "Deleted Creator";
                        return x;
                    });
                }
            }
            else
            {
                var hits = hitRepository.GetTopPatronContent(startDate, endDate, cognitoId);
                return hits.Select(x =>
                {
                    x.CreatorName = _creatorService.GetCreator(x.CreatorId)?.Username ?? "Deleted Creator";
                    return x;
                });
            }
            return new List<CreatorHitCount>();
        }

        public Guid? GetCreatorFromPartnerAndUri(Uri uri, Partner partner)
        {
            //TODO, what do with this user
            ///www.partner.com/stories

            ///www.partner.com/creator/pictures; www.partner.com/profile/creator; www.partner.com/stories/creator/
            ///www.partner.com\/profile\/?.*\.php
            var patterns = partner.UserPattern.Split(";");
            _logger.LogDebug($"Parter {partner.Sites} has {patterns.Length} patterns to try.");

            // iterate through the possible patterns
            foreach (var pattern in patterns)
            {
                var patternUri = new Uri(pattern);
                if (uri.Segments.Length < patternUri.Segments.Length)
                {
                    continue;
                }
                var regexPattern = "(?:" + pattern.Replace(Constants.CreatorPlaceholderKey, ")([^/]*)(?:", StringComparison.Ordinal) + ")";
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
                        if (group.Value != uri.ToString()
                            && !string.IsNullOrWhiteSpace(group.Value)
                            && !PartnerService.InvalidUsernameCharacters.Any(x => group.Value.Contains(x, StringComparison.Ordinal)))
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
