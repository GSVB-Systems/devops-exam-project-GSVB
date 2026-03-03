using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class UserDto
{
    [Required]
    public string UserId { get; set; } = default!;
    [Required]
    public string Username { get; set; } = default!;
    public string? DiscordUsername { get; set; }
    [Required]
    public string Email { get; set; } = default!;
}