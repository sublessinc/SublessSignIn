using System;
using Microsoft.EntityFrameworkCore.Design;

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
            var options = new DatabaseSettings()
            {
                ConnectionString = Environment.GetEnvironmentVariable("MigrationsGenerationConnectionString")

            };
            return new UserRepository(Microsoft.Extensions.Options.Options.Create(options));
        }
    }
}
