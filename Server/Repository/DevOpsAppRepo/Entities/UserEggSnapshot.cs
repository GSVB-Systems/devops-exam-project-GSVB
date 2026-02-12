namespace DevOpsAppRepo.Entities;

// Data fields to be added here as well as in EggSnapshotService.cs

public class UserEggSnapshot
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EiUserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Alt";
    public string? UserName { get; set; }
    public ulong? BoostsUsed { get; set; }
    public double? SoulEggs { get; set; }
    public ulong? EggsOfProphecy { get; set; }
    public ulong? TruthEggs { get; set; }
    public ulong? GoldenEggsEarned { get; set; }
    public ulong? GoldenEggsSpent { get; set; }
    public long? GoldenEggsBalance { get; set; }
    public double? CraftingXp { get; set; }
    public double? Mer { get; set; }
    public double? Jer { get; set; }
    public double? Cer { get; set; }
    public double? Eb { get; set; }
    public DateTime LastFetchedUtc { get; set; }
    public string RawJson { get; set; } = string.Empty;
}
