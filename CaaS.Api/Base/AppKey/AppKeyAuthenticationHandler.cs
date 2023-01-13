using System.Security.Claims;
using System.Text.Encodings.Web;
using CaaS.Core.Base;
using CaaS.Infrastructure.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ISystemClock = Microsoft.AspNetCore.Authentication.ISystemClock;

namespace CaaS.Api.Base.AppKey; 

public class AppKeyAuthenticationHandler : AuthenticationHandler<AppKeyAuthenticationOptions> {
    public AppKeyAuthenticationHandler(IOptionsMonitor<AppKeyAuthenticationOptions> options, ILoggerFactory logger, 
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null) {
            return AuthenticateResult.NoResult();
        }
        if (!Request.Headers.TryGetValue(HeaderConstants.AppKey, out var appKey)) {
            return AuthenticateResult.Fail("No app-key provided");
        }
        var appKeyValidator = Context.RequestServices.GetRequiredService<IAppKeyValidator>();
        if (!await appKeyValidator.ValidateAppKeyAsync(appKey)) {
            return AuthenticateResult.Fail("Invalid app-key provided");
        }
        var principal = new ClaimsPrincipal(new ClaimsIdentity(null, Scheme.Name));
        var ticket = new AuthenticationTicket(principal, null, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}