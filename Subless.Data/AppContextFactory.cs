using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using static Subless.Data.DataDi;

namespace Subless.Data
{
    public class AppContextFactory : IDesignTimeDbContextFactory<Repository>
    {
        public AppContextFactory()
        {
            // A parameter-less constructor is required by the EF Core CLI tools.
        }

        public Repository CreateDbContext(string[] args)
        {
            var credsJson = Environment.GetEnvironmentVariable("dbCreds");
            if (string.IsNullOrWhiteSpace(credsJson))
            {
                throw new ArgumentNullException("dbCreds must be provided to generate migrations");
            }
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(credsJson);


            var options = new DatabaseSettings()
            {
                ConnectionString = dbCreds.GetDatabaseConnection()
            };
            return new Repository(Microsoft.Extensions.Options.Options.Create(options), new LoggerFactory());
        }
    }
}
