using DevOpsAppContracts.Models;

namespace DevOpsAppService.Interfaces;

public interface IEggAccountService
{
    Task<IReadOnlyList<EggAccountDto>> GetForUserAsync(string userId);
    Task<EggAccountDto?> CreateAsync(string userId, CreateEggAccountDto dto);
    Task<EggAccountDto?> UpdateAsync(string userId, string id, UpdateEggAccountDto dto);
    Task<bool> DeleteAsync(string userId, string id);
}
