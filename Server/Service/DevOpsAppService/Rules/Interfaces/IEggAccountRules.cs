using DevOpsAppContracts.Models;

namespace DevOpsAppService.Rules.Interfaces;

public interface IEggAccountRules
{
    Task ValidateGetForUserAsync(string userId);
    Task ValidateCreateAsync(string userId, CreateEggAccountDto dto);
    Task ValidateUpdateAsync(string userId, string id, UpdateEggAccountDto dto);
    Task ValidateDeleteAsync(string userId, string id);
}

