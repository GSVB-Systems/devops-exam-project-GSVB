using DevOpsAppContracts.Models;

namespace DevOpsAppService.Interfaces;

public interface IEggSnapshotService
{
    Task<EggSnapshotResultDto?> FetchAndSaveAsync(string userId, CancellationToken cancellationToken = default);
}
