using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsAppApi.Controllers;

[ApiController]
[Route("api/leaderboards")]
public class LeaderboardsController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardsController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaderboardEntryDto>>> Get(CancellationToken cancellationToken)
    {
        var entries = await _leaderboardService.GetEntriesAsync(cancellationToken);
        return Ok(entries);
    }
}
