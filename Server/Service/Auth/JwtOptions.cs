namespace DevOpsAppService.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Secret { get; set; } = default!;
    public int ExpiresInMinutes { get; set; } = 60;
}

