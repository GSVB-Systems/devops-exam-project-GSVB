using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppService.Interfaces;

namespace Test.ServiceTests;

public class LeaderboardServiceTests(ILeaderboardService service, DevOpsAppDbContext ctx)
{
    [Fact]
    public async Task GetEntriesAsync_FiltersEmptyAndOrdersByEb()
    {
        var userId = await CreateUserAsync();

        ctx.UserEggSnapshots.AddRange(
            new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = "ei-1",
                Status = "Alt",
                UserName = "Alpha",
                Eb = 10,
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            },
            new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = "ei-2",
                Status = "Alt",
                UserName = "Beta",
                Eb = 20,
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            },
            new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = "ei-3",
                Status = "Alt",
                UserName = "NoStats",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            });
        await ctx.SaveChangesAsync();

        var results = await service.GetEntriesAsync();

        Assert.Equal(2, results.Count);
        Assert.Equal("Beta", results[0].UserName);
        Assert.Equal(20, results[0].Eb);
        Assert.Equal("Alpha", results[1].UserName);
        Assert.Equal(10, results[1].Eb);
    }

    private async Task<string> CreateUserAsync()
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "leader-user",
            Email = "leader-user@test.local",
            HashedPassword = "hash"
        };

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        return user.UserId;
    }
}
