using System;
using Subless.Models;

namespace Subless.Services
{
    public interface ICreatorService
    {
        void ActivateCreator(Guid userId, Guid activationCode);
        Creator GetCreator(Guid id);
        Creator GetCreatorByCognitoid(string cognitoId);
        Creator UpdateCreator(string cognitoId, Creator creator);
    }
}