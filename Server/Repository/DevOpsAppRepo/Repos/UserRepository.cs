using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;

namespace DevOpsAppRepo.Repos;

public class UserRepository : Repo<User>, IUserRepository
{
    public UserRepository(DevOpsAppDbContext context) : base(context)
    {
    }
}