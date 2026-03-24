using Bogus;
using DevOpsAppContracts.Models;

namespace Test.Builders;

public static class UserTestData
{
    private static readonly Faker<CreateUserDto> CreateUserFaker = new Faker<CreateUserDto>()
        .RuleFor(x => x.Username, f => f.Internet.UserName())
        .RuleFor(x => x.DiscordUsername, f => f.Random.Bool() ? f.Internet.UserName() : null)
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Password, f => f.Internet.Password(12, false));

    private static readonly Faker<UpdateUserDto> UpdateUserFaker = new Faker<UpdateUserDto>()
        .RuleFor(x => x.Username, f => f.Random.Bool() ? f.Internet.UserName() : null)
        .RuleFor(x => x.DiscordUsername, f => f.Random.Bool() ? f.Internet.UserName() : null)
        .RuleFor(x => x.Email, f => f.Random.Bool() ? f.Internet.Email() : null)
        .RuleFor(x => x.Password, f => f.Random.Bool() ? f.Internet.Password(12, false) : null);

    private static readonly Faker<LoginRequestDto> LoginRequestFaker = new Faker<LoginRequestDto>()
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Password, f => f.Internet.Password(12, false));

    private static readonly Faker<UserDto> UserFaker = new Faker<UserDto>()
        .RuleFor(x => x.UserId, f => f.Random.Guid().ToString("N"))
        .RuleFor(x => x.Username, f => f.Internet.UserName())
        .RuleFor(x => x.DiscordUsername, f => f.Random.Bool() ? f.Internet.UserName() : null)
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Role, _ => "User");

    private static readonly Faker<AuthResponseDto> AuthResponseFaker = new Faker<AuthResponseDto>()
        .RuleFor(x => x.AccessToken, f => f.Random.Guid().ToString("N"))
        .RuleFor(x => x.TokenType, _ => "Bearer")
        .RuleFor(x => x.ExpiresInSeconds, f => f.Random.Int(60, 3600));

    public static CreateUserDto CreateUser(int seed = 201) => CreateUserFaker.UseSeed(seed).Generate();
    public static UpdateUserDto UpdateUser(int seed = 202) => UpdateUserFaker.UseSeed(seed).Generate();
    public static LoginRequestDto LoginRequest(int seed = 203) => LoginRequestFaker.UseSeed(seed).Generate();
    public static UserDto User(int seed = 204) => UserFaker.UseSeed(seed).Generate();
    public static AuthResponseDto AuthResponse(int seed = 205) => AuthResponseFaker.UseSeed(seed).Generate();
}