using System.Threading.Tasks;

namespace SublessSignIn
{
    public interface IHealthCheck
    {
        Task<bool> IsHealthy();
    }
}