using DevOpsAppContracts.Models;

namespace DevOpsAppService.Interfaces;

public interface IEggSnapshotService
{
    Task<EggSnapshotResultDto?> FetchAndSaveAsync(
        string userId,
        string eiUserId,
        CancellationToken cancellationToken = default);
}
