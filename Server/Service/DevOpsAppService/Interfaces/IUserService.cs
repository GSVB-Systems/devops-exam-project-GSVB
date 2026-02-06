using DevOpsAppContracts.Models;

namespace DevOpsAppService.Interfaces;

public interface IUserService : IService<UserDto, CreateUserDto, UpdateUserDto>
{
    
}