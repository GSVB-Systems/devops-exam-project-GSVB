using Bogus;
using DevOpsAppContracts.Models;

namespace Test.Builders;

public static class ControllerTestData
{
    private static readonly Faker<UserDto> UserDtoFaker = new Faker<UserDto>()
        .RuleFor(x => x.UserId, f => f.Random.Guid().ToString("N"))
        .RuleFor(x => x.Username, f => f.Internet.UserName())
        .RuleFor(x => x.DiscordUsername, f => f.Random.Bool() ? f.Internet.UserName() : null)
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Role, _ => "User");

    private static readonly Faker<LoginRequestDto> LoginRequestFaker = new Faker<LoginRequestDto>()
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Password, f => f.Internet.Password(12, false));

    private static readonly Faker<AuthResponseDto> AuthResponseFaker = new Faker<AuthResponseDto>()
        .RuleFor(x => x.AccessToken, f => f.Random.Guid().ToString("N"))
        .RuleFor(x => x.TokenType, _ => "Bearer")
        .RuleFor(x => x.ExpiresInSeconds, f => f.Random.Int(300, 3600));

    private static readonly Faker<LeaderboardEntryDto> LeaderboardEntryFaker = new Faker<LeaderboardEntryDto>()
        .RuleFor(x => x.UserName, f => f.Internet.UserName())
        .RuleFor(x => x.Eb, f => f.Random.Double(1, 1000))
        .RuleFor(x => x.SoulEggs, f => f.Random.Double(1, 1000000))
        .RuleFor(x => x.EggsOfProphecy, f => (ulong)f.Random.Int(1, 200))
        .RuleFor(x => x.Mer, f => f.Random.Double(1, 200))
        .RuleFor(x => x.Jer, f => f.Random.Double(1, 200));

    private static readonly Faker<CreateEggAccountDto> CreateEggAccountFaker = new Faker<CreateEggAccountDto>()
        .RuleFor(x => x.EiUserId, f => $"ei-{f.Random.Int(1000, 9999)}")
        .RuleFor(x => x.Status, f => f.PickRandom("Main", "Alt"));

    private static readonly Faker<EggAccountRefreshDto> EggAccountRefreshFaker = new Faker<EggAccountRefreshDto>()
        .RuleFor(x => x.UserName, f => f.Internet.UserName())
        .RuleFor(x => x.SoulEggs, f => f.Random.Double(1, 1000000))
        .RuleFor(x => x.EggsOfProphecy, f => (ulong)f.Random.Int(1, 200))
        .RuleFor(x => x.TruthEggs, f => (ulong)f.Random.Int(1, 200))
        .RuleFor(x => x.GoldenEggsBalance, f => f.Random.Long(1000, 1000000))
        .RuleFor(x => x.Mer, f => f.Random.Double(1, 200))
        .RuleFor(x => x.Jer, f => f.Random.Double(1, 200))
        .RuleFor(x => x.Eb, f => f.Random.Double(1, 1000))
        .RuleFor(x => x.LastFetchedUtc, f => f.Date.Recent().ToUniversalTime())
        .RuleFor(x => x.NextAllowedFetchUtc, f => f.Date.Soon().ToUniversalTime())
        .RuleFor(x => x.WasFetched, _ => true);

    public static UserDto UserDto(int seed = 101) => UserDtoFaker.UseSeed(seed).Generate();

    public static LoginRequestDto LoginRequest(int seed = 102) => LoginRequestFaker.UseSeed(seed).Generate();

    public static AuthResponseDto AuthResponse(int seed = 103) => AuthResponseFaker.UseSeed(seed).Generate();

    public static IReadOnlyList<LeaderboardEntryDto> LeaderboardEntries(int count = 1, int seed = 104) =>
        LeaderboardEntryFaker.UseSeed(seed).Generate(count);

    public static CreateEggAccountDto CreateEggAccount(int seed = 105) => CreateEggAccountFaker.UseSeed(seed).Generate();

    public static EggAccountRefreshDto EggAccountRefresh(int seed = 106) => EggAccountRefreshFaker.UseSeed(seed).Generate();
}

