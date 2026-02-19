using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevOpsAppRepo.Entities;
using DevOpsAppService.Auth;

namespace Test.ServiceTests;

public class JwtTokenServiceTests(JwtTokenService tokenService, JwtOptions jwtOptions)
{
    [Fact]
    public void CreateToken_IncludesExpectedClaimsAndExpiry()
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "token-user",
            Email = "token-user@test.local"
        };

        var before = DateTime.UtcNow;
        var (token, expiresAtUtc) = tokenService.CreateToken(user);
        var after = DateTime.UtcNow;

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var sub = parsed.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        var email = parsed.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value;
        var name = parsed.Claims.Single(c => c.Type == ClaimTypes.Name).Value;

        Assert.Equal(user.UserId, sub);
        Assert.Equal(user.Email, email);
        Assert.Equal(user.Username, name);
        Assert.Equal(jwtOptions.Issuer, parsed.Issuer);
        Assert.Contains(jwtOptions.Audience, parsed.Audiences);
        Assert.InRange(expiresAtUtc, before.AddMinutes(jwtOptions.ExpiresInMinutes).AddSeconds(-5), after.AddMinutes(jwtOptions.ExpiresInMinutes).AddSeconds(5));
    }
}
