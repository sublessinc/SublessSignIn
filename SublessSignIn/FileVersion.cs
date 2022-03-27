using Subless.Services;
using System;
using System.Threading.Tasks;

namespace SublessSignIn
{
    public class FileVersion : IVersion
    {
        private readonly string version;

        public FileVersion()
        {
            try {
                this.version = System.IO.File.ReadAllText(@"version.txt");
            } catch {
                this.version = "";
            }
        }

        public async Task<string> GetVersion()
        {
            return await Task.FromResult(this.version);
        }
    }
}
