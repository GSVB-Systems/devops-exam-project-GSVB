using DevOpsAppContracts.Models;
using DevOpsAppService.Interfaces;
using DevOpsAppApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace DevOpsAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly FeatureFlagsOptions _featureFlags;

    public UserController(IUserService userService, IOptions<FeatureFlagsOptions> featureFlags)
    {
        _userService = userService;
        _featureFlags = featureFlags.Value;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        return user is null ? NotFound() : Ok(user);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe([FromBody] UpdateUserDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var updated = await _userService.UpdateAsync(userId, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [Authorize (Roles = "Admin")]
    [HttpGet("getAllUsers")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll([FromQuery] SieveModel? parameters)
    {
        if (!_featureFlags.Admin)
            return NotFound();

        var users = await _userService.GetAllAsync(parameters);
        return Ok(users);
    }

    [Authorize]
    [HttpGet("getUserById/{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        if (!_featureFlags.Admin)
            return NotFound();

        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("createUser")]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var created = await _userService.CreateAsync(dto);
        return Ok(created);
    }

    [Authorize]
    [HttpPut("updateUser/{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UpdateUserDto dto)
    {
        if (!_featureFlags.Admin)
            return NotFound();

        var updated = await _userService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [Authorize]
    [HttpDelete("deleteUser/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!_featureFlags.Admin)
            return NotFound();

        var deleted = await _userService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _userService.LoginAsync(dto);
        return result is null ? Unauthorized() : Ok(result);
    }
}