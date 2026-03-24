using DevOpsAppApi.Controllers;
using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.Builders;

namespace Test.ControllerTests;

public class EggControllersTests
{
    [Fact]
    public async Task EggAccount_GetForUser_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller);

        var result = await controller.GetForUser();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task EggAccount_Create_ReturnsBadRequest_WhenServiceReturnsNull()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        accountService.Setup(x => x.CreateAsync("u-1", It.IsAny<CreateEggAccountDto>())).ReturnsAsync((EggAccountDto?)null);
        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");
        var request = ControllerTestData.CreateEggAccount();

        var result = await controller.Create(request);

        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task EggAccount_Create_ReturnsOk_WhenServiceReturnsAccount()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        var request = ControllerTestData.CreateEggAccount(seed: 301);
        var created = new EggAccountDto { Id = "acc-1", EiUserId = request.EiUserId, Status = "Main" };

        accountService.Setup(x => x.CreateAsync("u-1", request)).ReturnsAsync(created);

        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Create(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(created, ok.Value);
    }

    [Fact]
    public async Task EggAccount_GetForUser_ReturnsOk_WhenAccountsExist()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        IReadOnlyList<EggAccountDto> accounts =
        [
            new EggAccountDto { Id = "acc-1", EiUserId = "ei-1", Status = "Main" }
        ];

        accountService.Setup(x => x.GetForUserAsync("u-1")).ReturnsAsync(accounts);

        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.GetForUser();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(accounts, ok.Value);
    }

    [Fact]
    public async Task EggAccount_Update_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller);

        var result = await controller.Update("acc-1", new UpdateEggAccountDto { Status = "Main" });

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task EggAccount_Update_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        var request = new UpdateEggAccountDto { Status = "Main" };

        accountService.Setup(x => x.UpdateAsync("u-1", "acc-1", request)).ReturnsAsync((EggAccountDto?)null);

        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Update("acc-1", request);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task EggAccount_Update_ReturnsOk_WhenServiceReturnsAccount()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        var request = new UpdateEggAccountDto { Status = "Alt" };
        var updated = new EggAccountDto { Id = "acc-1", EiUserId = "ei-1", Status = "Alt" };

        accountService.Setup(x => x.UpdateAsync("u-1", "acc-1", request)).ReturnsAsync(updated);

        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Update("acc-1", request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updated, ok.Value);
    }

    [Fact]
    public async Task EggAccount_Delete_ReturnsNoContent_WhenDeleted()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        accountService.Setup(x => x.DeleteAsync("u-1", "acc-1")).ReturnsAsync(true);
        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Delete("acc-1");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task EggAccount_Delete_ReturnsNotFound_WhenNotDeleted()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        accountService.Setup(x => x.DeleteAsync("u-1", "acc-1")).ReturnsAsync(false);
        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Delete("acc-1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EggAccount_Refresh_ReturnsNotFound_WhenNoSnapshot()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        snapshotService.Setup(x => x.FetchAndSaveAsync("u-1", "ei-1", CancellationToken.None)).ReturnsAsync((EggAccountRefreshDto?)null);
        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Refresh("ei-1", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task EggAccount_Refresh_ReturnsOk_WhenSnapshotExists()
    {
        var accountService = new Mock<IEggAccountService>();
        var snapshotService = new Mock<IEggSnapshotService>();
        var expected = ControllerTestData.EggAccountRefresh(seed: 302);

        snapshotService.Setup(x => x.FetchAndSaveAsync("u-1", "ei-1", CancellationToken.None)).ReturnsAsync(expected);

        var controller = new EggAccountController(accountService.Object, snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Refresh("ei-1", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task EggSnapshot_Refresh_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        var snapshotService = new Mock<IEggSnapshotService>();
        var controller = new EggSnapshotController(snapshotService.Object);
        ControllerTestHelpers.SetUser(controller);

        var result = await controller.Refresh("ei-1", CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task EggSnapshot_Refresh_ReturnsOk_WhenSnapshotExists()
    {
        var expected = ControllerTestData.EggAccountRefresh();
        var snapshotService = new Mock<IEggSnapshotService>();
        snapshotService.Setup(x => x.FetchAndSaveAsync("u-1", "ei-1", CancellationToken.None)).ReturnsAsync(expected);
        var controller = new EggSnapshotController(snapshotService.Object);
        ControllerTestHelpers.SetUser(controller, "u-1");

        var result = await controller.Refresh("ei-1", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }
}


