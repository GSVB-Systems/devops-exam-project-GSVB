using Bogus;
using Ei;

namespace Test.Builders;

public static class EggApiClientTestData
{
    private static readonly Faker<string> UserIdFaker = new Faker<string>()
        .CustomInstantiator(f => f.Random.Replace("ei-########"));

    private static readonly Faker<string> DeviceIdFaker = new Faker<string>()
        .CustomInstantiator(f => f.Random.Guid().ToString("N"));

    private static readonly Faker<ClientVersionHolder> ClientVersionFaker = new Faker<ClientVersionHolder>()
        .RuleFor(x => x.Value, f => f.Random.Int(100, 999));

    private static readonly Faker<EggIncFirstContactResponse> ResponseFaker = new Faker<EggIncFirstContactResponse>()
        .CustomInstantiator(f => new EggIncFirstContactResponse
        {
            Backup = new Backup
            {
                UserName = f.Internet.UserName()
            }
        });

    public static string UserId(int seed = 301) => UserIdFaker.UseSeed(seed).Generate();

    public static string DeviceId(int seed = 302) => DeviceIdFaker.UseSeed(seed).Generate();

    public static uint ClientVersion(int seed = 303) => (uint)ClientVersionFaker.UseSeed(seed).Generate().Value;

    public static EggIncFirstContactResponse Response(string userId)
    {
        var response = ResponseFaker.UseSeed(304).Generate();
        response.EiUserId = userId;
        response.Backup.EiUserId = userId;
        return response;
    }

    private sealed class ClientVersionHolder
    {
        public int Value { get; set; }
    }
}




