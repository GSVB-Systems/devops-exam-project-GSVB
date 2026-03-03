using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class CreateUserDto
{
    [Required]
    public string Username { get; set; } = default!;
    
    public string? DiscordUsername { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    [Required]
    public string Password { get; set; } = default!;
}