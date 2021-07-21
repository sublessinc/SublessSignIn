using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services
{
    public interface ICreatorService
    {
        void ActivateCreator(Guid userId, Guid activationCode);
        Creator GetCachedCreatorFromPartnerAndUsername(string username, Guid partnerId);
        Creator GetCreator(Guid id);
        Creator GetCreatorByCognitoid(string cognitoId);
        IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId);
        IEnumerable<MontlyPaymentStats> GetStatsForCreator(Creator creator);
        Creator UpdateCreator(string cognitoId, Creator creator);
    }
}