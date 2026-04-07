using System.Net;
using System.Text;
using DevOpsAppRepo;
using DevOpsAppService.EggApi;
using Ei;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Test.Builders;

namespace Test.ServiceTests;

public class EggApiClientTests(DevOpsAppDbContext ctx)
{
    [Fact]
    public async Task GetFirstContactAsync_UsesDefaultsAndConfiguredClientVersion()
    {
        Assert.NotNull(ctx);

        var userId = EggApiClientTestData.UserId(seed: 310);
        var configuredClientVersion = EggApiClientTestData.ClientVersion(seed: 311);
        var expectedResponse = EggApiClientTestData.Response(userId);
        var encodedResponse = Convert.ToBase64String(expectedResponse.ToByteArray());

        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(encodedResponse, Encoding.UTF8, "text/plain")
            });

        var client = CreateClient(handler, new EggApiOptions { AuxbrainBaseUrl = "https://egg.test", ClientVersion = configuredClientVersion });

        var result = await client.GetFirstContactAsync(userId, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(userId, result.EiUserId);
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/ei/bot_first_contact", handler.LastRequest!.RequestUri!.AbsolutePath);

        var requestPayload = ParseRequestPayload(handler.LastRequestBody!);
        Assert.Equal(userId, requestPayload.EiUserId);
        Assert.Equal(userId, requestPayload.UserId);
        Assert.Equal("red-blue", requestPayload.DeviceId);
        Assert.Equal(configuredClientVersion, requestPayload.ClientVersion);
        Assert.Equal(configuredClientVersion, requestPayload.Rinfo.ClientVersion);
    }

    [Fact]
    public async Task GetFirstContactAsync_UsesExplicitDeviceAndClientVersion()
    {
        var userId = EggApiClientTestData.UserId(seed: 320);
        var explicitVersion = EggApiClientTestData.ClientVersion(seed: 321);
        var explicitDeviceId = EggApiClientTestData.DeviceId(seed: 322);
        var expectedResponse = EggApiClientTestData.Response(userId);
        var encodedResponse = Convert.ToBase64String(expectedResponse.ToByteArray());

        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(encodedResponse, Encoding.UTF8, "text/plain")
            });

        var client = CreateClient(handler, new EggApiOptions { AuxbrainBaseUrl = "https://egg.test", ClientVersion = 999 });

        var result = await client.GetFirstContactAsync(
            userId,
            clientVersion: explicitVersion,
            deviceId: explicitDeviceId,
            cancellationToken: TestContext.Current.CancellationToken);

        var requestPayload = ParseRequestPayload(handler.LastRequestBody!);

        Assert.Equal(userId, result.EiUserId);
        Assert.Equal(explicitDeviceId, requestPayload.DeviceId);
        Assert.Equal(explicitVersion, requestPayload.ClientVersion);
        Assert.Equal(explicitVersion, requestPayload.Rinfo.ClientVersion);
    }

    [Fact]
    public async Task GetFirstContactAsync_InvalidResponseBase64_ThrowsInvalidOperationException()
    {
        var userId = EggApiClientTestData.UserId(seed: 330);
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not-base64", Encoding.UTF8, "text/plain")
            });

        var client = CreateClient(handler, new EggApiOptions { AuxbrainBaseUrl = "https://egg.test" });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            client.GetFirstContactAsync(userId, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains("Body: not-base64", ex.Message);
    }

    [Fact]
    public async Task GetFirstContactAsync_WhitespaceUserId_ThrowsArgumentException()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "text/plain")
            });

        var client = CreateClient(handler, new EggApiOptions { AuxbrainBaseUrl = "https://egg.test" });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetFirstContactAsync(" ", cancellationToken: TestContext.Current.CancellationToken));
    }

    private static EggApiClient CreateClient(HttpMessageHandler handler, EggApiOptions options)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.AuxbrainBaseUrl)
        };

        return new EggApiClient(httpClient, Options.Create(options));
    }

    private static EggIncFirstContactRequest ParseRequestPayload(string requestBody)
    {
        var key = "data=";
        var formEntry = requestBody
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Single(x => x.StartsWith(key, StringComparison.Ordinal));

        var base64Payload = WebUtility.UrlDecode(formEntry[key.Length..]);
        var payload = Convert.FromBase64String(base64Payload);
        return EggIncFirstContactRequest.Parser.ParseFrom(payload);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastRequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return responder(request);
        }
    }
}



