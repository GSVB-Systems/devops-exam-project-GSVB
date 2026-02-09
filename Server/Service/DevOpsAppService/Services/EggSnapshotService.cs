using DevOpsAppContracts.Models;
using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Interfaces;
using Google.Protobuf;
using System.Security.Cryptography;
using System.Text;

namespace DevOpsAppService.Services;

public class EggSnapshotService : IEggSnapshotService
{
    private static readonly TimeSpan MinFetchInterval = TimeSpan.FromMinutes(5);
    private readonly IEggApiClient _eggApiClient;
    private readonly IUserRepository _userRepository;
    private readonly IUserEggSnapshotRepository _snapshotRepository;

    public EggSnapshotService(
        IEggApiClient eggApiClient,
        IUserRepository userRepository,
        IUserEggSnapshotRepository snapshotRepository)
    {
        _eggApiClient = eggApiClient;
        _userRepository = userRepository;
        _snapshotRepository = snapshotRepository;
    }

    public async Task<EggSnapshotResultDto?> FetchAndSaveAsync(
        string userId,
        string eiUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;
        if (string.IsNullOrWhiteSpace(eiUserId))
            return null;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return null;

        var existing = await _snapshotRepository.GetByIdAsync(userId);
        var now = DateTime.UtcNow;

        if (existing is not null && now - existing.LastFetchedUtc < MinFetchInterval)
        {
            return new EggSnapshotResultDto
            {
                UserId = userId,
                EiUserIdHash = existing.EiUserIdHash,
                LastFetchedUtc = existing.LastFetchedUtc,
                NextAllowedFetchUtc = existing.LastFetchedUtc.Add(MinFetchInterval),
                WasFetched = false
            };
        }

        // Fetch new data from Egg API

        var response = await _eggApiClient.GetFirstContactAsync(eiUserId, cancellationToken: cancellationToken);
        var rawJson = JsonFormatter.Default.Format(response);
        var resolvedEiUserId = string.IsNullOrWhiteSpace(response.EiUserId)
            ? response.Backup?.EiUserId
            : response.EiUserId;
        if (string.IsNullOrWhiteSpace(resolvedEiUserId))
            resolvedEiUserId = eiUserId;

        var eiUserIdHash = HashEiUserId(resolvedEiUserId);
        var boostsUsed = response.Backup?.Stats?.HasBoostsUsed == true
            ? response.Backup.Stats.BoostsUsed
            : (ulong?)null;

        if (existing is null)
        {
            existing = new UserEggSnapshot
            {
                UserId = userId,
                EiUserIdHash = eiUserIdHash,
                BoostsUsed = boostsUsed,
                LastFetchedUtc = now,
                RawJson = rawJson
            };
            await _snapshotRepository.AddAsync(existing);
        }
        else
        {
            existing.EiUserIdHash = eiUserIdHash;
            existing.BoostsUsed = boostsUsed;
            existing.LastFetchedUtc = now;
            existing.RawJson = rawJson;
            await _snapshotRepository.UpdateAsync(existing);
        }

        await _snapshotRepository.SaveChangesAsync();

        return new EggSnapshotResultDto
        {
            UserId = userId,
            EiUserIdHash = existing.EiUserIdHash,
            LastFetchedUtc = existing.LastFetchedUtc,
            NextAllowedFetchUtc = existing.LastFetchedUtc.Add(MinFetchInterval),
            WasFetched = true
        };
    }

    private static string? HashEiUserId(string? eiUserId)
    {
        if (string.IsNullOrWhiteSpace(eiUserId))
            return null;

        var bytes = Encoding.UTF8.GetBytes(eiUserId);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

}
