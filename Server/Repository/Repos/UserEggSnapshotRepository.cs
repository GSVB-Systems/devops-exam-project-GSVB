using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAppRepo.Repos;

public class UserEggSnapshotRepository : Repo<UserEggSnapshot>, IUserEggSnapshotRepository
{
    public UserEggSnapshotRepository(DevOpsAppDbContext context) : base(context)
    {
    }

    public async Task<UserEggSnapshot?> GetByUserAndEiUserIdAsync(string userId, string eiUserId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.EiUserId == eiUserId);
    }

    public async Task<List<UserEggSnapshot>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Status == "Main")
            .ThenBy(s => s.UserName)
            .ToListAsync();
    }

    public async Task<UserEggSnapshot?> GetMainByUserIdAsync(string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Main");
    }
}
