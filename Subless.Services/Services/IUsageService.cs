using System;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IUsageService
    {
        void SaveUsage(UsageType type, Guid userId);
    }
}