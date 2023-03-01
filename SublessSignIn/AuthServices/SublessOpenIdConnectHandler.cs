using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

public class SublessOpenIdConnectHandler : OpenIdConnectHandler
{
    private readonly ILogger<SublessOpenIdConnectHandler> _logger;
    public SublessOpenIdConnectHandler(IOptionsMonitor<OpenIdConnectOptions> options, ILoggerFactory logger, HtmlEncoder htmlEncoder, UrlEncoder encoder, ISystemClock clock) : base(options, logger, htmlEncoder, encoder, clock)
    {
        _logger = logger.CreateLogger<SublessOpenIdConnectHandler>();
    }

    protected async override Task<HandleRequestResult> GetUserInformationAsync(OpenIdConnectMessage message, JwtSecurityToken jwt, ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        try
        {
            return await base.GetUserInformationAsync(message,jwt, principal, properties);
        }
        catch (TaskCanceledException e)
        {
            _logger.LogWarning("Task cancelled error occurred. This is occurring on logout in some cases.");
            _logger.LogWarning($"If the subject ID has been removed from the sessionDB, we can probably ignore this error");
            return HandleRequestResult.Fail(e.Message);
        }
    }
}
