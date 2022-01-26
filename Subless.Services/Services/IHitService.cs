using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services
{
    public interface IHitService
    {
        IEnumerable<Hit> GetCreatorHitsByDate(DateTime startDate, DateTime endDate, Guid creatorId);
        IEnumerable<Hit> GetHitsByDate(DateTime startDate, DateTime endDate, Guid userId);
        void SaveHit(string userId, Uri uri);
        Hit TestHit(string userId, Uri uri);
    }
}