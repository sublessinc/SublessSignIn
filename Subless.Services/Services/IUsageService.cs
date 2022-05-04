using System;
using Subless.Models;

namespace Subless.Services
{
    public interface IUsageService
    {
        void SaveUsage(UsageType type, Guid userId);
    }
}