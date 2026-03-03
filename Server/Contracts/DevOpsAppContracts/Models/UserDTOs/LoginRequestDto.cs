using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    [Required]
    public string Password { get; set; } = default!;
}

