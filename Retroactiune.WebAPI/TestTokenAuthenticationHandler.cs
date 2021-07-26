using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Retroactiune
{
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TestTokenAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {

        public TestTokenAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {new Claim("token", "allow_all")};
            var identity = new ClaimsIdentity(claims, nameof(TestTokenAuthenticationHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}