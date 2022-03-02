using Subless.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services
{
    public interface ICreatorService
    {
        Task ActivateCreator(Guid userId, Guid activationCode, string email);
        Task FireCreatorActivationWebhook(Creator creator, bool wasValid);
        Creator GetCachedCreatorFromPartnerAndUsername(string username, Guid partnerId);
        Creator GetCreator(Guid id);
        Creator GetCreatorByCognitoid(string cognitoId);
        IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId);
        IEnumerable<MontlyPaymentStats> GetStatsForCreator(Creator creator);
        Task UnlinkCreator(string cognitoId, Guid id);
        Task<Creator> UpdateCreator(string cognitoId, Creator creator);
    }
}