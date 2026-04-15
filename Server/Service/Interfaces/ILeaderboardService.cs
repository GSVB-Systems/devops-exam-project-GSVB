using DevOpsAppContracts.Models;

namespace DevOpsAppService.Interfaces;

public interface ILeaderboardService
{
    Task<IReadOnlyList<LeaderboardEntryDto>> GetEntriesAsync(CancellationToken cancellationToken = default);
}
