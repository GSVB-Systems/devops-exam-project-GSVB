using System.ComponentModel.DataAnnotations;

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
    [Required]
    public DateTime LastFetchedUtc { get; set; }
    [Required]
    public DateTime NextAllowedFetchUtc { get; set; }
    [Required]
    public bool WasFetched { get; set; }
}
