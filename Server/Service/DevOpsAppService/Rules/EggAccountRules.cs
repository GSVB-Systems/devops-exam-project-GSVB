using DevOpsAppContracts.Models;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Rules.Interfaces;
using service.Exceptions;

namespace DevOpsAppService.Rules;

public class EggAccountRules : IEggAccountRules
{
    private readonly IUserEggSnapshotRepository _snapshotRepository;

    public EggAccountRules(IUserEggSnapshotRepository snapshotRepository)
    {
        _snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public Task ValidateGetForUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidRequestException("Bruger ID skal medtages.");

        return Task.CompletedTask;
    }

    public Task ValidateCreateAsync(string userId, CreateEggAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidRequestException("Bruger ID skal medtages.");
        if (dto is null)
            throw new InvalidRequestException("CreateEggAccountDto skal medtages.");
        if (string.IsNullOrWhiteSpace(dto.EiUserId))
            throw new InvalidRequestException("Angiv EiUserId.");
        if (dto.Status is not null && !IsValidStatus(dto.Status))
            throw new RangeValidationException("Status skal være 'Main' eller 'Alt'.");

        return Task.CompletedTask;
    }

    public async Task ValidateUpdateAsync(string userId, string id, UpdateEggAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidRequestException("Bruger ID skal medtages.");
        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidRequestException("Konto ID skal medtages.");
        if (dto is null)
            throw new InvalidRequestException("UpdateEggAccountDto skal medtages.");
        if (dto.Status is not null && !IsValidStatus(dto.Status))
            throw new RangeValidationException("Status skal være 'Main' eller 'Alt'.");

        var existing = await _snapshotRepository.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            throw new ResourceNotFoundException("EggAccount findes ikke for denne bruger.");
    }

    public async Task ValidateDeleteAsync(string userId, string id)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidRequestException("Bruger ID skal medtages.");
        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidRequestException("Konto ID skal medtages.");

        var existing = await _snapshotRepository.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            throw new ResourceNotFoundException("EggAccount findes ikke for denne bruger.");
    }

    private static bool IsValidStatus(string status)
    {
        return status.Equals("Main", StringComparison.OrdinalIgnoreCase)
            || status.Equals("Alt", StringComparison.OrdinalIgnoreCase);
    }
}
