using DevOpsAppContracts.Models;
using DevOpsAppRepo.Entities;

namespace DevOpsAppService.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User entity)
    {
        return new UserDto
        {
            UserId = entity.UserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email
        };
    }

    public static User ToEntity(CreateUserDto dto)
    {
        return new User
        {
            UserId = Guid.NewGuid().ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            HashedPassword = string.Empty // set by UserService (hashed)
        };
    }

    public static void Apply(UpdateUserDto dto, User entity)
    {
        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        // Password hashing is handled in UserService; don't assign dto.Password here.
    }
}