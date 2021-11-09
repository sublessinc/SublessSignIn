using System.Threading.Tasks;

namespace Subless.PayoutCalculator
{
    public interface IHealthCheck
    {
        Task<bool> IsHealthy();
    }
}