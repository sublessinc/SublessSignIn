using System.Threading.Tasks;

namespace SublessSignIn
{
    public class FileVersion : IVersion
    {
        private readonly string version;

        public FileVersion()
        {
            try
            {
                version = System.IO.File.ReadAllText(@"version.txt");
            }
            catch
            {
                version = "";
            }
        }

        public async Task<string> GetVersion()
        {
            return await Task.FromResult(version);
        }
    }
}
