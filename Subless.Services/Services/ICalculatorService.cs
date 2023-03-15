using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface ICalculatorService
    {
        CalculatorResult CalculatePayoutsOverRange(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null);
        Guid QueueCalculation(DateTimeOffset startDate, DateTimeOffset endDate);
        IEnumerable<Payee> GetCreatorPayees(double payment, Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction);
        IEnumerable<Payee> GetPartnerPayees(double payment, Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction);
        void ExecutedQueuedCalculation();
        CalculatorResult GetQueuedResult(Guid id);
    }
}
