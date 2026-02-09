using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace DevOpsAppApi.Controllers;

[ApiController]
[Route("api/egg-snapshots")]
public class EggSnapshotController : ControllerBase
{
    private readonly IEggSnapshotService _snapshotService;

    public EggSnapshotController(IEggSnapshotService snapshotService)
    {
        _snapshotService = snapshotService;
    }

    [Authorize]
    [HttpPost("refresh/{eiUserId}")]
    public async Task<ActionResult<EggSnapshotResultDto>> Refresh(string eiUserId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _snapshotService.FetchAndSaveAsync(userId, eiUserId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
