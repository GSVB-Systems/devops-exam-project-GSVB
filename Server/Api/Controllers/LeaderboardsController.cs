using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using DevOpsAppApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DevOpsAppApi.Controllers;

[ApiController]
[Route("api/leaderboards")]
public class LeaderboardsController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly FeatureFlagsOptions _featureFlags;

    public LeaderboardsController(
        ILeaderboardService leaderboardService,
        IOptions<FeatureFlagsOptions> featureFlags)
    {
        _leaderboardService = leaderboardService;
        _featureFlags = featureFlags.Value;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaderboardEntryDto>>> Get(CancellationToken cancellationToken)
    {
        if (!_featureFlags.Leaderboards)
            return NotFound();

        var entries = await _leaderboardService.GetEntriesAsync(cancellationToken);
        return Ok(entries);
    }
}
