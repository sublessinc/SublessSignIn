using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Data;
using Subless.Services;

namespace Subless.PayoutCalculator
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IFileStorageService fileStorageService;
        private readonly IStripeService stripeService;
        private readonly IUserRepository userRepository;

        public HealthCheck(IFileStorageService fileStorageService,
                           IStripeService stripeService,
                           IUserRepository userRepository)
        {
            this.fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            this.stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<bool> IsHealthly()
        {
            var db = userRepository.CanAccessDatabase();
            var stripe = stripeService.CanAccessStripe();
            var s3 = fileStorageService.CanAccessS3();
            return (await db && await s3 && await stripe);
        }
    }
}
