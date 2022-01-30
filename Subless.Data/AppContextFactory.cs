using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Subless.Data.DataDi;

namespace Subless.Data
{
    public class AppContextFactory : IDesignTimeDbContextFactory<UserRepository>
    {
        public AppContextFactory()
        {
            // A parameter-less constructor is required by the EF Core CLI tools.
        }

        public UserRepository CreateDbContext(string[] args)
        {
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(Environment.GetEnvironmentVariable("dbCreds"));

                
            var options = new DatabaseSettings()
            {
                ConnectionString = dbCreds.GetDatabaseConnection()
            };
            return new UserRepository(Microsoft.Extensions.Options.Options.Create(options), new LoggerFactory());
        }
    }
}
