using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("{userId}/refresh")]
    public async Task<ActionResult<EggSnapshotResultDto>> Refresh(string userId, CancellationToken cancellationToken)
    {
        var result = await _snapshotService.FetchAndSaveAsync(userId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
