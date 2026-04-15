using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevOpsAppApi.Controllers;

[ApiController]
[Route("api/egg-accounts")]
public class EggAccountController : ControllerBase
{
    private readonly IEggAccountService _eggAccountService;
    private readonly IEggSnapshotService _snapshotService;

    public EggAccountController(IEggAccountService eggAccountService, IEggSnapshotService snapshotService)
    {
        _eggAccountService = eggAccountService;
        _snapshotService = snapshotService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EggAccountDto>>> GetForUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var accounts = await _eggAccountService.GetForUserAsync(userId);
        return Ok(accounts);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EggAccountDto>> Create([FromBody] CreateEggAccountDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var created = await _eggAccountService.CreateAsync(userId, dto);
        return created is null ? BadRequest() : Ok(created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<EggAccountDto>> Update(string id, [FromBody] UpdateEggAccountDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var updated = await _eggAccountService.UpdateAsync(userId, id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var deleted = await _eggAccountService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpPost("refresh/{eiUserId}")]
    public async Task<ActionResult<EggAccountRefreshDto>> Refresh(string eiUserId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _snapshotService.FetchAndSaveAsync(userId, eiUserId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
