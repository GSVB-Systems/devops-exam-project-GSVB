using DevOpsAppContracts.Models;
using Sieve.Models;

namespace DevOpsAppService.Interfaces;

public interface IService<T, TCreate, TUpdate> 
    where T : class
    where TCreate : class
    where TUpdate : class
{
    Task<T?> GetByIdAsync(string id);
    Task<PagedResult<T>> GetAllAsync(SieveModel? parameters);
    Task<T> CreateAsync(TCreate createDto);
    Task<T?> UpdateAsync(string id, TUpdate updateDto);
    Task<bool> DeleteAsync(string id);
}