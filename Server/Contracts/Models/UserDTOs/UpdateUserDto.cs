namespace DevOpsAppContracts.Models;

public sealed class UpdateUserDto
{
    public string? Username { get; set; }
    public string? DiscordUsername { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    
    public string? Role { get; set; }
}