using DevOpsAppRepo.Entities;

namespace DevOpsAppRepo.Interfaces;

public interface IUserEggSnapshotRepository : IRepository<UserEggSnapshot>
{
	Task<UserEggSnapshot?> GetByUserAndEiUserIdAsync(string userId, string eiUserId);
	Task<List<UserEggSnapshot>> GetByUserIdAsync(string userId);
	Task<UserEggSnapshot?> GetMainByUserIdAsync(string userId);
}
