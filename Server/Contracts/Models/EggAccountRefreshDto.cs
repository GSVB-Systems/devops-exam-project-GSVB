namespace DevOpsAppContracts.Models;

public class EggAccountRefreshDto
{
    public string? UserName { get; set; }
    public double? SoulEggs { get; set; }
    public ulong? EggsOfProphecy { get; set; }
    public ulong? TruthEggs { get; set; }
    public long? GoldenEggsBalance { get; set; }
    public double? Mer { get; set; }
    public double? Jer { get; set; }
    public double? Eb { get; set; }
    public DateTime LastFetchedUtc { get; set; }
    public DateTime NextAllowedFetchUtc { get; set; }
    public bool WasFetched { get; set; }
}
