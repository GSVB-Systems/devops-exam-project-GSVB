using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;

namespace DevOpsAppRepo.Repos;

public class UserEggSnapshotRepository : Repo<UserEggSnapshot>, IUserEggSnapshotRepository
{
    public UserEggSnapshotRepository(DevOpsAppDbContext context) : base(context)
    {
    }
}
