using DevOpsAppContracts.Models;
using DevOpsAppRepo;
using DevOpsAppService.Interfaces;

namespace Test;

public class CreateUserTest(IUserService userService, DevOpsAppDbContext ctx)
{

    [Fact]
    public async Task CreateUserValid()
    {
        var user = new CreateUserDto()
        {
            FirstName = "Lars",
            LastName = "Hansen",
            Email = "Lars@test.it",
            Password = "JegKanHuskeDenneKode123!"
        };

        await userService.CreateAsync(user);

        Assert.Equal(1, ctx.Users.Count());
    }

}