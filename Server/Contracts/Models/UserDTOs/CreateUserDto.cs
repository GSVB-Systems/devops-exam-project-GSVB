namespace DevOpsAppContracts.Models;

public sealed class CreateUserDto
{
    public string Username { get; set; } = default!;
    public string? DiscordUsername { get; set; }
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}