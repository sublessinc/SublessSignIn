using System.Threading.Tasks;

namespace SublessSignIn
{
    public interface IVersion
    {
        public Task<string> GetVersion();
    }
}