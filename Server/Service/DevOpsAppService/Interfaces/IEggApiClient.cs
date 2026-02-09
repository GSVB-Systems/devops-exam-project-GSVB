using System.Text.Json;

namespace DevOpsAppService.Interfaces;

public interface IEggApiClient
{
    Task<JsonDocument> GetFirstContactAsync(
        string userId,
        uint? clientVersion = null,
        string? deviceId = null,
        CancellationToken cancellationToken = default);
}
