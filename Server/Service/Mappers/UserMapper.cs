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
            Username = entity.Username,
            DiscordUsername = entity.DiscordUsername,
            Email = entity.Email,
            Role = entity.Role
        };
    }

    public static User ToEntity(CreateUserDto dto)
    {
        return new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = dto.Username,
            DiscordUsername = dto.DiscordUsername,
            Email = dto.Email,
            HashedPassword = string.Empty, // set by UserService (hashed)
            
        };
    }

    public static void Apply(UpdateUserDto dto, User entity)
    {
        if (dto.Username is not null) entity.Username = dto.Username;
        if (dto.DiscordUsername is not null) entity.DiscordUsername = dto.DiscordUsername;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.Role is not null) entity.Role = dto.Role;
        // Password hashing is handled in UserService; don't assign dto.Password here.
    }
}