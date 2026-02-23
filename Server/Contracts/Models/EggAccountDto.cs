namespace DevOpsAppContracts.Models;

public sealed class EggAccountDto
{
    public string Id { get; set; } = default!;
    public string EiUserId { get; set; } = default!;
    public string Status { get; set; } = "Alt";
    public string? UserName { get; set; }
    public DateTime? LastFetchedUtc { get; set; }
}
