using Bogus;
using DevOpsAppContracts.Models;

namespace Test.Builders;

public static class UserTestData
{
    public static readonly Faker<CreateUserDto> CreateUserFaker = new Faker<CreateUserDto>()
        .RuleFor(x => x.FirstName, f => f.Name.FirstName())
        .RuleFor(x => x.LastName, f => f.Name.LastName())
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Password, f => f.Internet.Password(12, false));

    public static readonly Faker<UpdateUserDto> UpdateUserFaker = new Faker<UpdateUserDto>()
        .RuleFor(x => x.FirstName, f => f.Random.Bool() ? f.Name.FirstName() : null)
        .RuleFor(x => x.LastName, f => f.Random.Bool() ? f.Name.LastName() : null)
        .RuleFor(x => x.Email, f => f.Random.Bool() ? f.Internet.Email() : null)
        .RuleFor(x => x.Password, f => f.Random.Bool() ? f.Internet.Password(12, false) : null);

    public static readonly Faker<LoginRequestDto> LoginRequestFaker = new Faker<LoginRequestDto>()
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Password, f => f.Internet.Password(12, false));

    public static readonly Faker<UserDto> UserFaker = new Faker<UserDto>()
        .RuleFor(x => x.UserId, f => f.Random.Guid().ToString("N"))
        .RuleFor(x => x.FirstName, f => f.Name.FirstName())
        .RuleFor(x => x.LastName, f => f.Name.LastName())
        .RuleFor(x => x.Email, f => f.Internet.Email());

    public static readonly Faker<AuthResponseDto> AuthResponseFaker = new Faker<AuthResponseDto>()
        .RuleFor(x => x.AccessToken, f => f.Random.Guid().ToString("N"))
        .RuleFor(x => x.TokenType, _ => "Bearer")
        .RuleFor(x => x.ExpiresInSeconds, f => f.Random.Int(60, 3600));

    public static CreateUserDto CreateUser() => CreateUserFaker.Generate();
    public static UpdateUserDto UpdateUser() => UpdateUserFaker.Generate();
    public static LoginRequestDto LoginRequest() => LoginRequestFaker.Generate();
    public static UserDto User() => UserFaker.Generate();
    public static AuthResponseDto AuthResponse() => AuthResponseFaker.Generate();
}