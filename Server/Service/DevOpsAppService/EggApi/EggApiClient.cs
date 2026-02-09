using System.Text.Json;
using DevOpsAppService.Interfaces;
using Ei;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace DevOpsAppService.EggApi;

public sealed class EggApiClient : IEggApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EggApiOptions _options;

    public EggApiClient(HttpClient httpClient, IOptions<EggApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    public async Task<JsonDocument> GetFirstContactAsync(
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
            UserId = userId
        };

        if (!string.IsNullOrWhiteSpace(deviceId))
            request.DeviceId = deviceId;

        var resolvedClientVersion = clientVersion ?? _options.ClientVersion;
        if (resolvedClientVersion.HasValue)
            request.ClientVersion = resolvedClientVersion.Value;

        var payload = request.ToByteArray();
        using var content = new ByteArrayContent(payload);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-protobuf");

        using var response = await _httpClient.PostAsync("/ei/bot_first_contact", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var protoResponse = EggIncFirstContactResponse.Parser.ParseFrom(responseStream);
        var json = JsonFormatter.Default.Format(protoResponse);
        return JsonDocument.Parse(json);
    }
}
