using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public sealed class CreateEggAccountDto
{
    [Required]
    public string EiUserId { get; set; } = default!;
    public string? Status { get; set; }
}
