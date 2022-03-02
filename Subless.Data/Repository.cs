using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Data
{
    public partial class Repository
    {
        ILogger<Repository> logger { get; set; }
        private readonly IOptions<DatabaseSettings> _options;

        public Repository(IOptions<DatabaseSettings> options, ILoggerFactory loggerFactory)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = loggerFactory.CreateLogger<Repository>();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_options.Value.ConnectionString);
        }

        public void LogDbStats()
        {
            logger.LogWarning($"Partners count: {Partners.Count()}");
            logger.LogWarning($"Creators count: {Creators.Count()}");
            logger.LogWarning($"User count: {Users.Count()}");
            logger.LogWarning($"Hits count: {Hits.Count()}");
            logger.LogWarning($"Payments count: {Payments.Count()}");
            logger.LogWarning($"PaymentAuditLogs count: {PaymentAuditLogs.Count()}");
        }
    }
}
