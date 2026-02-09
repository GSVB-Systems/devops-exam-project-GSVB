using DevOpsAppRepo.Entities;
using System.Threading.Tasks;

namespace DevOpsAppRepo.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}