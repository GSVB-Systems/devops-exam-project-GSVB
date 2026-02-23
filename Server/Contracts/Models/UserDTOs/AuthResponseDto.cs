namespace DevOpsAppContracts.Models;

public sealed class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresInSeconds { get; set; }
}

