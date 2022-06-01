using Subless.Services.Services;
using System;
using System.Threading.Tasks;

namespace SublessSignIn
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IAdministrationService administrationService;
        private readonly IStripeService stripeService;

        public HealthCheck(IAdministrationService administrationService, IStripeService stripeService)
        {
            this.administrationService = administrationService ?? throw new ArgumentNullException(nameof(administrationService));
            this.stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
        }

        public async Task<bool> IsHealthy()
        {
            return await CanAccessDatabase() && await CanAccessStripe();
        }
        private async Task<bool> CanAccessDatabase()
        {
            return await administrationService.CanAccessDatabase();
        }

        private async Task<bool> CanAccessStripe()
        {
            return await stripeService.CanAccessStripe();
        }
    }
}
