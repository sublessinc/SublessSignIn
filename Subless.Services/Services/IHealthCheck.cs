using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface IHealthCheck
    {
        Task<bool> IsHealthy();
    }
}