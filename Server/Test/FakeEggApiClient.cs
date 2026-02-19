using DevOpsAppService.Interfaces;
using Ei;
using System.Threading;

namespace Test;

public sealed class FakeEggApiClient : IEggApiClient
{
    private int _callCount;

    public int CallCount => _callCount;
    public EggIncFirstContactResponse NextResponse { get; private set; } = new();

    public void Reset(EggIncFirstContactResponse response)
    {
        Interlocked.Exchange(ref _callCount, 0);
        NextResponse = response;
    }

    public Task<EggIncFirstContactResponse> GetFirstContactAsync(
        string userId,
        uint? clientVersion = null,
        string? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        return Task.FromResult(NextResponse);
    }
}
