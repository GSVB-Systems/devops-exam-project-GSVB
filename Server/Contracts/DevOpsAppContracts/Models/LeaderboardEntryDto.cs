namespace DevOpsAppContracts.Models;

public sealed class LeaderboardEntryDto
{
    public string EiUserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Alt";
    public string? UserName { get; set; }
    public double? Eb { get; set; }
    public double? SoulEggs { get; set; }
    public ulong? EggsOfProphecy { get; set; }
    public double? Mer { get; set; }
    public double? Jer { get; set; }
    public DateTime LastFetchedUtc { get; set; }
}
