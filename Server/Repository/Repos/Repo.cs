using DevOpsAppRepo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAppRepo.Repos;

public class Repo<T> : IRepository<T> where T : class
{
    protected readonly DevOpsAppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repo(DevOpsAppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }


    public virtual async Task<T?> GetByIdAsync(string id)
    {
        
        return await _dbSet.FindAsync((object)id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<T> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}