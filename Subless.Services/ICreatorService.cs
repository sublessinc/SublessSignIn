using System;

namespace Subless.Services
{
    public interface ICreatorService
    {
        void ActivateCreator(Guid userId, Guid activationCode);
    }
}