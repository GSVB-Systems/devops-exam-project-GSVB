using DevOpsAppApi.Controllers;
using DevOpsAppApi;
using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Test.Builders;

namespace Test.ControllerTests;

public class UserControllerTests
{
    private static IOptions<FeatureFlagsOptions> EnabledFlags() => GetFlags(admin: true);
    private static IOptions<FeatureFlagsOptions> DisabledAdminFlags() => GetFlags(admin: false);

    private static IOptions<FeatureFlagsOptions> GetFlags(bool admin = true, bool leaderboards = true)
    {
        return Options.Create(new FeatureFlagsOptions
        {
            Admin = admin,
            Leaderboards = leaderboards
        });
    }

    private (UserController Controller, Mock<IUserService> Service) CreateController(IOptions<FeatureFlagsOptions>? flags = null)
    {
        var service = new Mock<IUserService>();
        var controller = new UserController(service.Object, flags ?? EnabledFlags());
        return (controller, service);
    }

    [Fact]
    public async Task GetMe_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        var (controller, _) = CreateController();
        ControllerTestHelpers.SetUser(controller);

        var result = await controller.GetMe();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var (controller, service) = CreateController();
        service.Setup(x => x.GetByIdAsync("u-1")).ReturnsAsync((UserDto?)null);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.GetMe();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WhenUserExists()
    {
        var (controller, service) = CreateController();
        var expected = ControllerTestData.UserDto();
        expected.UserId = "u-1";
        service.Setup(x => x.GetByIdAsync("u-1")).ReturnsAsync(expected);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.GetMe();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var (controller, _) = CreateController(DisabledAdminFlags());

        var result = await controller.GetAll(parameters: null);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var (controller, _) = CreateController(DisabledAdminFlags());

        var result = await controller.GetById("u-1");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserMissing()
    {
        var (controller, service) = CreateController();
        service.Setup(x => x.GetByIdAsync("u-1")).ReturnsAsync((UserDto?)null);

        var result = await controller.GetById("u-1");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var (controller, _) = CreateController(DisabledAdminFlags());
        var dto = new UpdateUserDto { Username = "updated-user" };

        var result = await controller.Update("u-1", dto);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenUserUpdated()
    {
        var (controller, service) = CreateController();
        var dto = new UpdateUserDto
        {
            Username = "updated-user",
            Email = "updated@test.local"
        };
        var expected = ControllerTestData.UserDto(seed: 109);
        service.Setup(x => x.UpdateAsync("u-1", dto)).ReturnsAsync(expected);

        var result = await controller.Update("u-1", dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var (controller, _) = CreateController(DisabledAdminFlags());

        var result = await controller.Delete("u-1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        var (controller, service) = CreateController();
        service.Setup(x => x.DeleteAsync("u-1")).ReturnsAsync(true);

        var result = await controller.Delete("u-1");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotDeleted()
    {
        var (controller, service) = CreateController();
        service.Setup(x => x.DeleteAsync("u-1")).ReturnsAsync(false);

        var result = await controller.Delete("u-1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsInvalid()
    {
        var (controller, service) = CreateController();
        var request = ControllerTestData.LoginRequest();
        service.Setup(x => x.LoginAsync(request)).ReturnsAsync((AuthResponseDto?)null);

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsValid()
    {
        var (controller, service) = CreateController();
        var expected = ControllerTestData.AuthResponse();
        var request = ControllerTestData.LoginRequest(seed: 107);
        service.Setup(x => x.LoginAsync(request)).ReturnsAsync(expected);

        var result = await controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }
}
