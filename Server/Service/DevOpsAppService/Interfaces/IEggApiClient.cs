using Ei;

namespace DevOpsAppService.Interfaces;

public interface IEggApiClient
{
    Task<EggIncFirstContactResponse> GetFirstContactAsync(
        string userId,
        uint? clientVersion = null,
        string? deviceId = null,
        CancellationToken cancellationToken = default);
}
