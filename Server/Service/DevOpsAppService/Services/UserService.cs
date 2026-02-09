using DevOpsAppContracts.Models;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Auth;
using DevOpsAppService.Interfaces;
using DevOpsAppService.Mappers;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace DevOpsAppService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISieveProcessor _sieve;
    private readonly PasswordService _passwordService;
    private readonly JwtTokenService _jwtTokenService;

    public UserService(IUserRepository userRepository, ISieveProcessor sieve, PasswordService passwordService, JwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _sieve = sieve;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var entity = await _userRepository.GetByIdAsync(id);
        return entity is null ? null : UserMapper.ToDto(entity);
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(SieveModel? parameters)
    {
        var sieveModel = parameters ?? new UserQueryParameters();

        var query = _userRepository.AsQueryable().AsNoTracking();

        // Match the example: count is on the unprocessed query.
        var totalCount = await query.CountAsync();

        var processedQuery = _sieve.Apply(sieveModel, query);
        var users = await processedQuery.ToListAsync();

        return new PagedResult<UserDto>
        {
            Items = users.Select(UserMapper.ToDto).ToList(),
            TotalCount = totalCount,
            Page = sieveModel.Page ?? 1,
            PageSize = sieveModel.PageSize ?? users.Count
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        if (createDto is null) throw new ArgumentNullException(nameof(createDto));
        if (string.IsNullOrWhiteSpace(createDto.Password))
            throw new ArgumentException("Password is required.", nameof(createDto));

        var entity = UserMapper.ToEntity(createDto);
        entity.HashedPassword = _passwordService.HashPassword(createDto.Password);

        await _userRepository.AddAsync(entity);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(entity);
    }

    public async Task<UserDto?> UpdateAsync(string id, UpdateUserDto updateDto)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;
        if (updateDto is null) throw new ArgumentNullException(nameof(updateDto));

        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        UserMapper.Apply(updateDto, existing);

        if (updateDto.Password is not null)
        {
            if (string.IsNullOrWhiteSpace(updateDto.Password))
                throw new ArgumentException("Password cannot be empty when provided.", nameof(updateDto));

            existing.HashedPassword = _passwordService.HashPassword(updateDto.Password);
        }

        await _userRepository.UpdateAsync(existing);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        await _userRepository.DeleteAsync(existing);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return null;

        var valid = _passwordService.VerifyPassword(request.Password, user.HashedPassword);
        if (!valid)
            return null;

        var (token, expiresAtUtc) = _jwtTokenService.CreateToken(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresInSeconds = (int)(expiresAtUtc - DateTime.UtcNow).TotalSeconds
        };
    }
}