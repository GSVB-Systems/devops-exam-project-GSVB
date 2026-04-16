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
    private static IOptions<FeatureFlagsOptions> EnabledFlags()
    {
        return Options.Create(new FeatureFlagsOptions
        {
            Admin = true,
            Leaderboards = true
        });
    }

    private static IOptions<FeatureFlagsOptions> DisabledAdminFlags()
    {
        return Options.Create(new FeatureFlagsOptions
        {
            Admin = false,
            Leaderboards = true
        });
    }

    [Fact]
    public async Task GetMe_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        var service = new Mock<IUserService>();
        var controller = new UserController(service.Object, EnabledFlags());
        ControllerTestHelpers.SetUser(controller);

        var result = await controller.GetMe();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var service = new Mock<IUserService>();
        service.Setup(x => x.GetByIdAsync("u-1")).ReturnsAsync((UserDto?)null);

        var controller = new UserController(service.Object, EnabledFlags());
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.GetMe();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WhenUserExists()
    {
        var service = new Mock<IUserService>();
        var expected = ControllerTestData.UserDto();
        expected.UserId = "u-1";
        service.Setup(x => x.GetByIdAsync("u-1")).ReturnsAsync(expected);

        var controller = new UserController(service.Object, EnabledFlags());
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.GetMe();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var service = new Mock<IUserService>();
        var controller = new UserController(service.Object, DisabledAdminFlags());

        var result = await controller.GetAll(parameters: null);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var service = new Mock<IUserService>();
        var controller = new UserController(service.Object, DisabledAdminFlags());

        var result = await controller.GetById("u-1");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserMissing()
    {
        var service = new Mock<IUserService>();
        service.Setup(x => x.GetByIdAsync("u-1")).ReturnsAsync((UserDto?)null);
        var controller = new UserController(service.Object, EnabledFlags());

        var result = await controller.GetById("u-1");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var service = new Mock<IUserService>();
        var controller = new UserController(service.Object, DisabledAdminFlags());
        var dto = new UpdateUserDto
        {
            Username = "updated-user"
        };

        var result = await controller.Update("u-1", dto);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenUserUpdated()
    {
        var service = new Mock<IUserService>();
        var dto = new UpdateUserDto
        {
            Username = "updated-user",
            Email = "updated@test.local"
        };
        var expected = ControllerTestData.UserDto(seed: 109);
        service.Setup(x => x.UpdateAsync("u-1", dto)).ReturnsAsync(expected);
        var controller = new UserController(service.Object, EnabledFlags());

        var result = await controller.Update("u-1", dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenAdminFlagDisabled()
    {
        var service = new Mock<IUserService>();
        var controller = new UserController(service.Object, DisabledAdminFlags());

        var result = await controller.Delete("u-1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        var service = new Mock<IUserService>();
        service.Setup(x => x.DeleteAsync("u-1")).ReturnsAsync(true);
        var controller = new UserController(service.Object, EnabledFlags());

        var result = await controller.Delete("u-1");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotDeleted()
    {
        var service = new Mock<IUserService>();
        service.Setup(x => x.DeleteAsync("u-1")).ReturnsAsync(false);
        var controller = new UserController(service.Object, EnabledFlags());

        var result = await controller.Delete("u-1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsInvalid()
    {
        var service = new Mock<IUserService>();
        var request = ControllerTestData.LoginRequest();
        service.Setup(x => x.LoginAsync(request)).ReturnsAsync((AuthResponseDto?)null);
        var controller = new UserController(service.Object, EnabledFlags());

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsValid()
    {
        var service = new Mock<IUserService>();
        var expected = ControllerTestData.AuthResponse();
        var request = ControllerTestData.LoginRequest(seed: 107);
        service.Setup(x => x.LoginAsync(request)).ReturnsAsync(expected);
        var controller = new UserController(service.Object, EnabledFlags());

        var result = await controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

}