using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using static Subless.Data.DataDi;

namespace Subless.Data
{
    public class DataProtectionContextFactory : IDesignTimeDbContextFactory<KeyStorageContext>
    {
        public DataProtectionContextFactory()
        {
            // A parameter-less constructor is required by the EF Core CLI tools.
        }

        public KeyStorageContext CreateDbContext(string[] args)
        {
            var credsJson = Environment.GetEnvironmentVariable("dbCreds");
            if (string.IsNullOrWhiteSpace(credsJson))
            {
                throw new ArgumentNullException("dbCreds must be provided to generate migrations");
            }
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(credsJson);

            var optionsBuilder = new DbContextOptionsBuilder<KeyStorageContext>();
            optionsBuilder.UseNpgsql(dbCreds.GetDatabaseConnection());
            return new KeyStorageContext(optionsBuilder.Options);
        }
    }
}
