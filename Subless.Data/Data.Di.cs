using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Subless.Data
{
    public static class DataDi
    {
        public static IServiceCollection RegisterDataDi(IServiceCollection services)
        {
            var json = Environment.GetEnvironmentVariable("dbCreds") ?? throw new ArgumentNullException("dbCreds");
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(json);
            services.Configure<DatabaseSettings>(options =>
            {
                options.ConnectionString = dbCreds.GetDatabaseConnection();
            });
            services.AddTransient<IUserRepository, Repository>();
            services.AddTransient<Repository, Repository>();
            services.AddTransient<IPaymentRepository, Repository>();
            services.AddTransient<ICreatorRepository, Repository>();
            services.AddTransient<IPartnerRepository, Repository>();
            services.AddTransient<IHitRepository, Repository>();
            services.AddTransient<IUsageRepository, Repository>();
            services.AddDbContext<Repository>(options => options.UseNpgsql(dbCreds.GetDatabaseConnection()));

            return services;
        }


        public class DbCreds
        {
            public string dbInstanceIdentifier;
            public string engine;
            public string host;
            public int port;
            public string resourceId;
            public string username;
            public string password;
            public string GetDatabaseConnection()
            {
                return $"Server={host}; " + $"Port={port}; " +
                    $"User Id={username};" + $"Password={password};" + $"Database={dbInstanceIdentifier};";
            }
        }

    }
}
