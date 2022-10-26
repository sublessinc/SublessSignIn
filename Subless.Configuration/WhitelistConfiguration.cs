using Microsoft.Extensions.DependencyInjection;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Configuration
{
    public class WhitelistConfiguration
    {

        public static IServiceCollection RegisterWhitelistConfig(IServiceCollection services)
        {
            services.Configure<UriWhitelist>(options =>
            {
                var unsplitList = Environment.GetEnvironmentVariable("UriWhitelist") ?? throw new ArgumentNullException("UriWhitelist");
                options.WhitelistedUris = unsplitList.Split(';').ToList();
            });

            return services;
        }
    }
}
