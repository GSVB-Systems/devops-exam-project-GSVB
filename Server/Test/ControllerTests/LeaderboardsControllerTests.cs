using DevOpsAppApi.Controllers;
using DevOpsAppApi;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Test.Builders;

namespace Test.ControllerTests;

public class LeaderboardsControllerTests
{
    private static IOptions<FeatureFlagsOptions> EnabledFlags()
    {
        return Options.Create(new FeatureFlagsOptions
        {
            Admin = true,
            Leaderboards = true
        });
    }

    [Fact]
    public async Task Get_ReturnsOk_WithEntries()
    {
        var service = new Mock<ILeaderboardService>();
        var expected = ControllerTestData.LeaderboardEntries(count: 2);
        service.Setup(x => x.GetEntriesAsync(CancellationToken.None)).ReturnsAsync(expected);
        var controller = new LeaderboardsController(service.Object, EnabledFlags());

        var result = await controller.Get(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }
}