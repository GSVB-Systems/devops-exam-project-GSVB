using DevOpsAppContracts.Models;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Interfaces;
using Sieve.Models;

namespace DevOpsAppService.Services;

public class Service<T, TCreate, TUpdate> : IService<T, TCreate, TUpdate>
    where T : class
    where TCreate : class
    where TUpdate : class
{
    protected readonly IRepository<T> Repository;

    public Service(IRepository<T> repository)
    {
        Repository = repository;
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public virtual async Task<PagedResult<T>> GetAllAsync(SieveModel? parameters = null)
    {
        var items = (await Repository.GetAllAsync())?.ToList() ?? new List<T>();
        var count = items.Count;

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = count,
            Page = 1,
            PageSize = count
        };
    }
    
    public virtual Task<T> CreateAsync(TCreate createDto)
    {
        throw new NotImplementedException($"CreateAsync({typeof(TCreate).Name}) must be implemented in the derived service.");
    }

    
    public virtual Task<T?> UpdateAsync(string id, TUpdate updateDto)
    {
        throw new NotImplementedException($"UpdateAsync({typeof(TUpdate).Name}) must be implemented in the derived service.");
    }
    
    
    public virtual async Task<bool> DeleteAsync(string id)
    {
        var existing = await Repository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await Repository.DeleteAsync(existing);
        await Repository.SaveChangesAsync();
        return true;
    }
}