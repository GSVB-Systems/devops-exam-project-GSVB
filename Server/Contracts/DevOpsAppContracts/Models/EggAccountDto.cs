using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class EggAccountDto
{
    [Required]
    public string Id { get; set; } = default!;
    [Required]
    public string EiUserId { get; set; } = default!;
    [Required]
    public string Status { get; set; } = "Alt";
    public string? UserName { get; set; }
    public DateTime? LastFetchedUtc { get; set; }
}
