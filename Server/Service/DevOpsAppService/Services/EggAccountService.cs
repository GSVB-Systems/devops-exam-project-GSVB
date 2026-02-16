using DevOpsAppContracts.Models;
using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAppService.Services;

public class EggAccountService : IEggAccountService
{
    private readonly IUserEggSnapshotRepository _snapshotRepository;

    public EggAccountService(IUserEggSnapshotRepository snapshotRepository)
    {
        _snapshotRepository = snapshotRepository;
    }

    public async Task<IReadOnlyList<EggAccountDto>> GetForUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Array.Empty<EggAccountDto>();

        var accounts = await _snapshotRepository.GetByUserIdAsync(userId);
        return accounts.Select(ToDto).ToList();
    }

    public async Task<EggAccountDto?> CreateAsync(string userId, CreateEggAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.EiUserId))
            throw new ArgumentException("EiUserId is required.", nameof(dto));

        var normalizedStatus = NormalizeStatus(dto.Status) ?? "Alt";
        var existing = await _snapshotRepository.GetByUserAndEiUserIdAsync(userId, dto.EiUserId);
        if (existing is not null)
        {
            if (normalizedStatus == "Main" && existing.Status != "Main")
            {
                await SetMainAsync(userId, existing);
            }

            return ToDto(existing);
        }

        var mainAccount = await _snapshotRepository.GetMainByUserIdAsync(userId);
        if (mainAccount is null)
        {
            normalizedStatus = "Main";
        }

        var entity = new UserEggSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EiUserId = dto.EiUserId,
            Status = normalizedStatus,
            LastFetchedUtc = DateTime.UtcNow,
            RawJson = "{}"
        };

        await _snapshotRepository.AddAsync(entity);
        await _snapshotRepository.SaveChangesAsync();

        if (entity.Status == "Main")
        {
            await SetMainAsync(userId, entity);
        }

        return ToDto(entity);
    }

    public async Task<EggAccountDto?> UpdateAsync(string userId, string id, UpdateEggAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(id))
            return null;
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var existing = await _snapshotRepository.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            return null;

        var normalizedStatus = NormalizeStatus(dto.Status);
        if (normalizedStatus is not null)
        {
            existing.Status = normalizedStatus;
            if (normalizedStatus == "Main")
            {
                await SetMainAsync(userId, existing);
            }
        }

        await _snapshotRepository.UpdateAsync(existing);
        await _snapshotRepository.SaveChangesAsync();

        return ToDto(existing);
    }

    public async Task<bool> DeleteAsync(string userId, string id)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(id))
            return false;

        var existing = await _snapshotRepository.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            return false;

        var wasMain = existing.Status == "Main";
        await _snapshotRepository.DeleteAsync(existing);
        await _snapshotRepository.SaveChangesAsync();

        if (wasMain)
        {
            var next = await _snapshotRepository.AsQueryable()
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.EiUserId)
                .FirstOrDefaultAsync();

            if (next is not null)
            {
                next.Status = "Main";
                await _snapshotRepository.UpdateAsync(next);
                await _snapshotRepository.SaveChangesAsync();
            }
        }

        return true;
    }

    private async Task SetMainAsync(string userId, UserEggSnapshot mainAccount)
    {
        var accounts = await _snapshotRepository.AsQueryable()
            .Where(s => s.UserId == userId)
            .ToListAsync();

        foreach (var account in accounts)
        {
            account.Status = account.Id == mainAccount.Id ? "Main" : "Alt";
        }

        await _snapshotRepository.SaveChangesAsync();
    }

    private static string? NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        return status.Equals("Main", StringComparison.OrdinalIgnoreCase)
            ? "Main"
            : status.Equals("Alt", StringComparison.OrdinalIgnoreCase)
                ? "Alt"
                : null;
    }

    private static EggAccountDto ToDto(UserEggSnapshot entity)
    {
        return new EggAccountDto
        {
            Id = entity.Id,
            EiUserId = entity.EiUserId,
            Status = entity.Status,
            UserName = entity.UserName,
            LastFetchedUtc = entity.LastFetchedUtc == default ? null : entity.LastFetchedUtc
        };
    }
}
