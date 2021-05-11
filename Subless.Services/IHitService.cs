using System;

namespace Subless.Services
{
    public interface IHitService
    {
        void SaveHit(string userId, Uri uri);
    }
}