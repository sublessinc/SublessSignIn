using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface ICreatorService
    {
        Task ActivateCreator(Guid userId, Guid activationCode, string email);
        IEnumerable<Guid> FilterInactiveCreators(IEnumerable<Guid> creatorIds);
        Task FireCreatorActivationWebhook(Creator creator, bool wasValid);
        Creator GetCachedCreatorFromPartnerAndUsername(string username, Guid partnerId);
        Creator GetCreator(Guid id);
        IEnumerable<Creator> GetCreatorsByCognitoid(string cognitoId);
        IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId);
        IEnumerable<MonthlyPaymentStats> GetStatsForCreator(Creator creator);
        Task UnlinkCreator(string cognitoId, Guid id);
        Task<Creator> UpdateCreatorPaymentInfo(string cognitoId, CreatorViewModel creator);
        void AcceptTerms(string cognitoId);
        IEnumerable<Creator>? GetCreatorOrDefaultByCognitoid(string cognitoId);
        IEnumerable<Creator> GetActiveCreators(IEnumerable<Guid> excludeCreators);
        CreatorMessage SetCreatorMessage(Guid creatorId, string message);
        CreatorMessage GetCreatorMessage(Guid creatorId);
        List<string> GetUriWhitelist();
        bool ActivationCodeValid(Guid activationCode);
    }
}
