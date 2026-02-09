using DevOpsAppService.Interfaces;
using Ei;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace DevOpsAppService.EggApi;

public sealed class EggApiClient : IEggApiClient
{
    private const string DefaultDeviceId = "red-blue";
    private readonly HttpClient _httpClient;
    private readonly EggApiOptions _options;

    public EggApiClient(HttpClient httpClient, IOptions<EggApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    public async Task<EggIncFirstContactResponse> GetFirstContactAsync(
        string userId,
        uint? clientVersion = null,
        string? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID is required.", nameof(userId));

        var request = new EggIncFirstContactRequest
        {
            EiUserId = userId,
            UserId = userId,
            Rinfo = new BasicRequestInfo
            {
                EiUserId = userId,
                ClientVersion = 999,
                Platform = "IOS",
                Build = "111331",
                Version = "1.35.4"
            }
        };

        request.DeviceId = string.IsNullOrWhiteSpace(deviceId) ? DefaultDeviceId : deviceId;

        var resolvedClientVersion = clientVersion ?? _options.ClientVersion;
        if (resolvedClientVersion.HasValue)
        {
            request.ClientVersion = resolvedClientVersion.Value;
            request.Rinfo.ClientVersion = resolvedClientVersion.Value;
        }

        var responseBytes = await PostProtoBase64Async("/ei/bot_first_contact", request, cancellationToken);
        return EggIncFirstContactResponse.Parser.ParseFrom(responseBytes);
    }

    private async Task<byte[]> PostProtoBase64Async(string path, IMessage request, CancellationToken cancellationToken)
    {
        var payload = request.ToByteArray();
        var base64Payload = Convert.ToBase64String(payload);
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["data"] = base64Payload
        });

        using var response = await _httpClient.PostAsync(path, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            return Convert.FromBase64String(responseText.Trim());
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException($"Body: {responseText}", ex);
        }
    }
}
