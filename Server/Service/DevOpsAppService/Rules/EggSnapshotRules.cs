using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Rules.Interfaces;
using service.Exceptions;

namespace DevOpsAppService.Rules;

public class EggSnapshotRules : IEggSnapshotRules
{
    private readonly IUserRepository _userRepository;
    private readonly IUserEggSnapshotRepository _snapshotRepository;

    public EggSnapshotRules(IUserRepository userRepository, IUserEggSnapshotRepository snapshotRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public async Task ValidateFetchAndSaveAsync(string userId, string eiUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidRequestException("Bruger ID skal medtages.");
        if (string.IsNullOrWhiteSpace(eiUserId))
            throw new InvalidRequestException("Angiv EiUserId.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new ResourceNotFoundException($"Bruger med ID '{userId}' findes ikke.");
    }
}
