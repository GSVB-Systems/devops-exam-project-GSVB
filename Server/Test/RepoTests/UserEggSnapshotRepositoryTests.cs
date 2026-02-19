using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;

namespace Test.RepoTests;

public class UserEggSnapshotRepositoryTests(IUserEggSnapshotRepository repo, DevOpsAppDbContext ctx)
{
    [Fact]
    public async Task GetByUserAndEiUserIdAsync_ReturnsMatch()
    {
        var userId = await CreateUserAsync();
        var snapshot = new UserEggSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EiUserId = "ei-match",
            Status = "Alt",
            LastFetchedUtc = DateTime.UtcNow,
            RawJson = "{}"
        };

        ctx.UserEggSnapshots.Add(snapshot);
        await ctx.SaveChangesAsync();

        var found = await repo.GetByUserAndEiUserIdAsync(userId, "ei-match");

        Assert.NotNull(found);
        Assert.Equal(snapshot.Id, found!.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_OrdersMainThenName()
    {
        var userId = await CreateUserAsync();
        ctx.UserEggSnapshots.AddRange(
            new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = "ei-c",
                Status = "Alt",
                UserName = "Charlie",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            },
            new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = "ei-a",
                Status = "Main",
                UserName = "Alpha",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            },
            new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = "ei-b",
                Status = "Alt",
                UserName = "Beta",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            });
        await ctx.SaveChangesAsync();

        var results = await repo.GetByUserIdAsync(userId);

        Assert.Equal(3, results.Count);
        Assert.Equal("Main", results[0].Status);
        Assert.Equal("Alpha", results[0].UserName);
        Assert.Equal("Beta", results[1].UserName);
        Assert.Equal("Charlie", results[2].UserName);
    }

    [Fact]
    public async Task GetMainByUserIdAsync_ReturnsMain()
    {
        var userId = await CreateUserAsync();
        var main = new UserEggSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EiUserId = "ei-main",
            Status = "Main",
            LastFetchedUtc = DateTime.UtcNow,
            RawJson = "{}"
        };

        ctx.UserEggSnapshots.Add(main);
        await ctx.SaveChangesAsync();

        var found = await repo.GetMainByUserIdAsync(userId);

        Assert.NotNull(found);
        Assert.Equal(main.Id, found!.Id);
    }

    private async Task<string> CreateUserAsync()
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "snapshot-user",
            Email = "snapshot-user@test.local",
            HashedPassword = "hash"
        };

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        return user.UserId;
    }
}
