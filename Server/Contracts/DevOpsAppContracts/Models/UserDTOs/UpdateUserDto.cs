using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class UpdateUserDto
{
    public string? Username { get; set; }
    public string? DiscordUsername { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    public string? Password { get; set; }
}