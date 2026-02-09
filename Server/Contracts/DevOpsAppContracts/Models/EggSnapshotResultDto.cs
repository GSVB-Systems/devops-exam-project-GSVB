namespace DevOpsAppContracts.Models;

public class EggSnapshotResultDto
{
    public string UserId { get; set; } = string.Empty;
    public string? EiUserIdHash { get; set; }
    public DateTime LastFetchedUtc { get; set; }
    public DateTime NextAllowedFetchUtc { get; set; }
    public bool WasFetched { get; set; }
}
