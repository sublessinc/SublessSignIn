using Subless.Models;
using System;
using System.Collections.Generic;

namespace Subless.Data
{
    public interface ICreatorRepository
    {
        void DeleteCreator(Creator creator);
        Creator GetCreator(Guid id);
        Creator GetCreatorByActivationCode(Guid code);
        Creator GetCreatorByUsernameAndPartnerId(string username, Guid partnerId);
        IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId);
        void SaveCreator(Creator creator);
        void UpdateCreator(Creator creator);
        void UpsertCreator(Creator creator);
        IEnumerable<Guid> FilterInvalidCreators(IEnumerable<Guid> creatorIds);

    }
}