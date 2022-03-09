using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Linq;

namespace Subless.Data
{
    public partial class Repository
    {
        ILogger<Repository> logger { get; set; }
        private readonly IOptions<DatabaseSettings> _options;
        public DbSet<SublessUserSession> UserSessions { get; set; }
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
