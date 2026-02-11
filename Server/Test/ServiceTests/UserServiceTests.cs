using DevOpsAppRepo;
using DevOpsAppService.Interfaces;
using Test.Builders;

namespace Test.ServiceTests;

public class UserServiceTests(IUserService userService, DevOpsAppDbContext ctx)
{
    
    [Fact]
    public async Task CreateUserValid()
    {
        var user = UserTestData.CreateUser();

        await userService.CreateAsync(user);

        Assert.Equal(1, ctx.Users.Count());
    }
    
    [Fact]
    public async Task LoginValid()
    {
        var user = UserTestData.CreateUser();
        await userService.CreateAsync(user);

        var loginRequest = UserTestData.LoginRequestFaker
            .Clone()
            .RuleFor(x => x.Email, _ => user.Email)
            .RuleFor(x => x.Password, _ => user.Password)
            .Generate();

        var auth = await userService.LoginAsync(loginRequest);

        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.AccessToken));
    }

    [Fact]
    public async Task LoginInvalid_ReturnsNull()
    {
        var user = UserTestData.CreateUser();
        await userService.CreateAsync(user);

        var loginRequest = UserTestData.LoginRequestFaker
            .Clone()
            .RuleFor(x => x.Email, _ => user.Email)
            .RuleFor(x => x.Password, _ => "WrongPassword123!")
            .Generate();

        var auth = await userService.LoginAsync(loginRequest);

        Assert.Null(auth);
    }

    [Fact]
    public async Task LoginEdgeCase_WhitespacePassword_ReturnsNull()
    {
        var user = UserTestData.CreateUser();
        await userService.CreateAsync(user);

        var loginRequest = UserTestData.LoginRequestFaker
            .Clone()
            .RuleFor(x => x.Email, _ => user.Email)
            .RuleFor(x => x.Password, _ => " ")
            .Generate();

        var auth = await userService.LoginAsync(loginRequest);

        Assert.Null(auth);
    }
}