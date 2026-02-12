using DevOpsAppContracts.Models;

namespace DevOpsAppService.Interfaces;

public interface IEggSnapshotService
{
    Task<EggAccountRefreshDto?> FetchAndSaveAsync(
        string userId,
        string eiUserId,
        CancellationToken cancellationToken = default);
}
