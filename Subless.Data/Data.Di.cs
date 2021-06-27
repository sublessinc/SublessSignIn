using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Subless.Data
{
    public static class DataDi
    {
        public static IServiceCollection RegisterDataDi(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(options =>
            {
                options.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new ArgumentNullException("CONNECTION_STRING");
            });
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<UserRepository, UserRepository>();
            services.AddDbContext<UserRepository>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
            return services;
        }
    }
}
