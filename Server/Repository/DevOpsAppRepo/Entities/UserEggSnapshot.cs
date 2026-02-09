namespace DevOpsAppRepo.Entities;

// Data fields to be added here as well as in EggSnapshotService.cs

public class UserEggSnapshot
{
    public string UserId { get; set; } = string.Empty;
    public string? EiUserIdHash { get; set; }
    public ulong? BoostsUsed { get; set; }
    public DateTime LastFetchedUtc { get; set; }
    public string RawJson { get; set; } = string.Empty;
}
