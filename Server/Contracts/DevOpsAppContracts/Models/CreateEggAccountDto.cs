namespace DevOpsAppContracts.Models;

public sealed class CreateEggAccountDto
{
    public string EiUserId { get; set; } = default!;
    public string? Status { get; set; }
}
