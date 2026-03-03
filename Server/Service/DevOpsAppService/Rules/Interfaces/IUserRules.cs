using DevOpsAppContracts.Models;
using Sieve.Models;

namespace DevOpsAppService.Rules.Interfaces;

public interface IUserRules
{
    Task ValidateCreateAsync(CreateUserDto createDto);
    Task ValidateUpdateAsync(string id, UpdateUserDto updateDto);
    Task ValidateDeleteAsync(string id);
    Task ValidateGetByIdAsync(string id);
    Task ValidateGetAllAsync(SieveModel? parameters);
}