using DevOpsAppApi.Controllers;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.Builders;

namespace Test.ControllerTests;

public class LeaderboardsControllerTests
{
    [Fact]
    public async Task Get_ReturnsOk_WithEntries()
    {
        var service = new Mock<ILeaderboardService>();
        var expected = ControllerTestData.LeaderboardEntries(count: 2);
        service.Setup(x => x.GetEntriesAsync(CancellationToken.None)).ReturnsAsync(expected);
        var controller = new LeaderboardsController(service.Object);

        var result = await controller.Get(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }
}