using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class AuthResponseDto
{
    [Required]
    public string AccessToken { get; set; } = default!;
    [Required]
    public string TokenType { get; set; } = "Bearer";
    [Required]
    public int ExpiresInSeconds { get; set; }
}

