using DevOpsAppContracts.Models;
using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppService.Interfaces;

namespace Test.ServiceTests;

public class EggAccountServiceTests(IEggAccountService service, DevOpsAppDbContext ctx)
{
    [Fact]
    public async Task CreateAsync_NoExistingMain_AssignsMain()
    {
        var userId = await CreateUserAsync();
        var dto = new CreateEggAccountDto
        {
            EiUserId = "ei-main",
            Status = "Alt"
        };

        var created = await service.CreateAsync(userId, dto);

        Assert.NotNull(created);
        Assert.Equal("Main", created!.Status);

        var entity = ctx.UserEggSnapshots.Single(s => s.UserId == userId && s.EiUserId == "ei-main");
        Assert.Equal("Main", entity.Status);
    }

    [Fact]
    public async Task UpdateAsync_PromotesToMainAndDemotesPrevious()
    {
        var userId = await CreateUserAsync();
        var mainId = Guid.NewGuid().ToString();
        var altId = Guid.NewGuid().ToString();

        ctx.UserEggSnapshots.AddRange(
            new UserEggSnapshot
            {
                Id = mainId,
                UserId = userId,
                EiUserId = "ei-a",
                Status = "Main",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            },
            new UserEggSnapshot
            {
                Id = altId,
                UserId = userId,
                EiUserId = "ei-b",
                Status = "Alt",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            });
        await ctx.SaveChangesAsync();

        var updated = await service.UpdateAsync(userId, altId, new UpdateEggAccountDto { Status = "Main" });

        Assert.NotNull(updated);
        Assert.Equal("Main", updated!.Status);

        var refreshedMain = ctx.UserEggSnapshots.Single(s => s.Id == mainId);
        var refreshedAlt = ctx.UserEggSnapshots.Single(s => s.Id == altId);
        Assert.Equal("Alt", refreshedMain.Status);
        Assert.Equal("Main", refreshedAlt.Status);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeletingMain_PromotesNextByEiUserId()
    {
        var userId = await CreateUserAsync();
        var mainId = Guid.NewGuid().ToString();
        var otherId = Guid.NewGuid().ToString();

        ctx.UserEggSnapshots.AddRange(
            new UserEggSnapshot
            {
                Id = mainId,
                UserId = userId,
                EiUserId = "ei-b",
                Status = "Main",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            },
            new UserEggSnapshot
            {
                Id = otherId,
                UserId = userId,
                EiUserId = "ei-a",
                Status = "Alt",
                LastFetchedUtc = DateTime.UtcNow,
                RawJson = "{}"
            });
        await ctx.SaveChangesAsync();

        var deleted = await service.DeleteAsync(userId, mainId);

        Assert.True(deleted);
        var remaining = ctx.UserEggSnapshots.Single(s => s.Id == otherId);
        Assert.Equal("Main", remaining.Status);
    }

    private async Task<string> CreateUserAsync()
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "egg-user",
            Email = "egg-user@test.local",
            HashedPassword = "hash"
        };

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        return user.UserId;
    }
}
