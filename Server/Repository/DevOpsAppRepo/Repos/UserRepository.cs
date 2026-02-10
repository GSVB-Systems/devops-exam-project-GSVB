using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAppRepo.Repos;

public class UserRepository : Repo<User>, IUserRepository
{
    public UserRepository(DevOpsAppDbContext context) : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}