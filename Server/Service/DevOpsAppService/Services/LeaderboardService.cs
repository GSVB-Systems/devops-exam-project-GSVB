using DevOpsAppContracts.Models;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAppService.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IUserEggSnapshotRepository _snapshotRepository;

    public LeaderboardService(IUserEggSnapshotRepository snapshotRepository)
    {
        _snapshotRepository = snapshotRepository;
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>> GetEntriesAsync(CancellationToken cancellationToken = default)
    {
        var query = _snapshotRepository.AsQueryable()
            .AsNoTracking()
            .Where(snapshot => snapshot.Eb.HasValue
                               || snapshot.SoulEggs.HasValue
                               || snapshot.EggsOfProphecy.HasValue
                               || snapshot.Mer.HasValue
                               || snapshot.Jer.HasValue)
            .Select(snapshot => new LeaderboardEntryDto
            {
                EiUserId = snapshot.EiUserId,
                Status = snapshot.Status,
                UserName = snapshot.UserName,
                Eb = snapshot.Eb,
                SoulEggs = snapshot.SoulEggs,
                EggsOfProphecy = snapshot.EggsOfProphecy,
                Mer = snapshot.Mer,
                Jer = snapshot.Jer,
                LastFetchedUtc = snapshot.LastFetchedUtc
            })
            .OrderByDescending(entry => entry.Eb ?? 0d);

        return await query.ToListAsync(cancellationToken);
    }
}
