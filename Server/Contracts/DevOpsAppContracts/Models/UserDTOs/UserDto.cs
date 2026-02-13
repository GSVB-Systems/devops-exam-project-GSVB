namespace DevOpsAppContracts.Models;

public sealed class UserDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string? DiscordUsername { get; set; }
    public string Email { get; set; } = default!;
    
    public string Role { get; set; } = default!;
}