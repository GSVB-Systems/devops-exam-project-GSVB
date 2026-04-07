using DevOpsAppRepo;
using DevOpsAppService.Interfaces;
using Test.Builders;

namespace Test.ServiceTests;

public class UserServiceTests(IUserService userService, DevOpsAppDbContext ctx)
{
    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var user = UserTestData.CreateUser(seed: 230);
        var created = await userService.CreateAsync(user);

        var found = await userService.GetByIdAsync(created.UserId);

        Assert.NotNull(found);
        Assert.Equal(created.UserId, found!.UserId);
        Assert.Equal(user.Email, found.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhitespaceId_ReturnsNull()
    {
        var found = await userService.GetByIdAsync(" ");

        Assert.Null(found);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCreatedUsers()
    {
        await userService.CreateAsync(UserTestData.CreateUser(seed: 231));
        await userService.CreateAsync(UserTestData.CreateUser(seed: 232));

        var result = await userService.GetAllAsync(parameters: null);

        Assert.True(result.TotalCount >= 2);
        Assert.True(result.Items.Count >= 2);
    }

    
    [Fact]
    public async Task CreateUserValid()
    {
        var user = UserTestData.CreateUser();

        await userService.CreateAsync(user);

        Assert.Equal(1, ctx.Users.Count());
    }
    
    [Fact]
    
    public async Task UpdateUserValid()
    {
        var user = UserTestData.CreateUser();
        var CreatedUser = await userService.CreateAsync(user);

        var updateDto = UserTestData.UpdateUser(seed: 210);
        updateDto.Email = user.Email;
        updateDto.Username = user.Username;

        var updatedUser = await userService.UpdateAsync(CreatedUser.UserId,updateDto);

        Assert.NotNull(updatedUser);
        Assert.Equal(updateDto.Username, updatedUser!.Username);
        Assert.Equal(updateDto.Email, updatedUser.Email);
    }

    [Fact]
    public async Task UpdateUserMissing_ReturnsNull()
    {
        var updateDto = UserTestData.UpdateUser(seed: 233);

        var updatedUser = await userService.UpdateAsync(Guid.NewGuid().ToString("N"), updateDto);

        Assert.Null(updatedUser);
    }
    
    [Fact]
    public async Task DeleteUserValid()
    {
        var user = UserTestData.CreateUser();
        var createdUser = await userService.CreateAsync(user);

        var deleted = await userService.DeleteAsync(createdUser.UserId);

        Assert.True(deleted);
        Assert.False(ctx.Users.Any(u => u.UserId == createdUser.UserId));
    }
    
    [Fact]
    public async Task LoginValid()
    {
        var user = UserTestData.CreateUser();
        await userService.CreateAsync(user);

        var loginRequest = UserTestData.LoginRequest(seed: 220);
        loginRequest.Email = user.Email;
        loginRequest.Password = user.Password;

        var auth = await userService.LoginAsync(loginRequest);

        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.AccessToken));
    }

    [Fact]
    public async Task LoginInvalid_ReturnsNull()
    {
        var user = UserTestData.CreateUser();
        await userService.CreateAsync(user);

        var loginRequest = UserTestData.LoginRequest(seed: 221);
        loginRequest.Email = user.Email;
        loginRequest.Password = "WrongPassword123!";

        var auth = await userService.LoginAsync(loginRequest);

        Assert.Null(auth);
    }

    [Fact]
    public async Task LoginEdgeCase_WhitespacePassword_ReturnsNull()
    {
        var user = UserTestData.CreateUser();
        await userService.CreateAsync(user);

        var loginRequest = UserTestData.LoginRequest(seed: 222);
        loginRequest.Email = user.Email;
        loginRequest.Password = " ";

        var auth = await userService.LoginAsync(loginRequest);

        Assert.Null(auth);
    }
}