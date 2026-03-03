namespace DevOpsAppService.Rules.Interfaces;

public interface IEggSnapshotRules
{
    Task ValidateFetchAndSaveAsync(string userId, string eiUserId, CancellationToken cancellationToken = default);
}

