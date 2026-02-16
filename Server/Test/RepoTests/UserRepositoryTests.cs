using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;

namespace Test.RepoTests;

public class UserRepositoryTests(IUserRepository repo, DevOpsAppDbContext ctx)
{
    [Fact]
    public async Task GetByEmailAsync_ReturnsUser()
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "repo-user",
            Email = "repo-user@test.local",
            HashedPassword = "hash"
        };

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var found = await repo.GetByEmailAsync(user.Email);

        Assert.NotNull(found);
        Assert.Equal(user.UserId, found!.UserId);
    }
}
