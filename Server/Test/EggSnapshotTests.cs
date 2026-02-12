using DevOpsAppContracts.Models;
using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppService.Interfaces;
using Ei;

namespace Test;

public class EggSnapshotTests(
    IEggSnapshotService snapshotService, IUserService userService, DevOpsAppDbContext ctx, FakeEggApiClient fakeEggApiClient)
{
    [Fact]
    public async Task FetchAndSaveAsync_CreatesSnapshotAndReturnsDto()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("lars");
        var response = BuildResponse("ei-123", boostsUsed: 42);
        fakeEggApiClient.Reset(response);

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-123", ct);

        Assert.NotNull(result);
        Assert.True(result!.WasFetched);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(result.LastFetchedUtc.AddMinutes(5), result.NextAllowedFetchUtc);
        Assert.Equal("ei-123", result.EiUserId);
        Assert.Equal("TestName", result.UserName);
        Assert.Equal(1234D, result.SoulEggs);
        Assert.Equal((ulong)7, result.EggsOfProphecy);
        Assert.Equal((ulong)5, result.TruthEggs);
        Assert.Equal(750, result.GoldenEggsBalance);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId);
        Assert.Equal("ei-123", snapshot.EiUserId);
        Assert.Equal("TestName", snapshot.UserName);
        Assert.Equal((ulong)42, snapshot.BoostsUsed);
        Assert.Equal(1234D, snapshot.SoulEggs);
        Assert.Equal((ulong)7, snapshot.EggsOfProphecy);
        Assert.Equal((ulong)5, snapshot.TruthEggs);
        Assert.Equal((ulong)1000, snapshot.GoldenEggsEarned);
        Assert.Equal((ulong)250, snapshot.GoldenEggsSpent);
        Assert.Equal(750, snapshot.GoldenEggsBalance);
        Assert.Equal(12.5D, snapshot.CraftingXp);
        Assert.False(string.IsNullOrWhiteSpace(snapshot.RawJson));
    }

    [Fact]
    public async Task FetchAndSaveAsync_WhenWithinMinInterval_ReturnsCached()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("ida");
        var lastFetchedUtc = DateTime.UtcNow;

        ctx.UserEggSnapshots.Add(new UserEggSnapshot
        {
            UserId = userId,
            EiUserId = "ei-previous",
            UserName = "CachedUser",
            BoostsUsed = 1,
            SoulEggs = 55,
            EggsOfProphecy = 2,
            TruthEggs = 3,
            GoldenEggsEarned = 100,
            GoldenEggsSpent = 25,
            GoldenEggsBalance = 75,
            CraftingXp = 2.5,
            LastFetchedUtc = lastFetchedUtc,
            RawJson = "{\"ok\":true}"
        });
        await ctx.SaveChangesAsync(ct);

        fakeEggApiClient.Reset(BuildResponse("ei-new", boostsUsed: 99));

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-new", ct);

        Assert.NotNull(result);
        Assert.False(result!.WasFetched);
        Assert.Equal("ei-previous", result.EiUserId);
        Assert.Equal("CachedUser", result.UserName);
        Assert.Equal(55D, result.SoulEggs);
        Assert.Equal((ulong)2, result.EggsOfProphecy);
        Assert.Equal((ulong)3, result.TruthEggs);
        Assert.Equal(75, result.GoldenEggsBalance);
        Assert.Equal(lastFetchedUtc, result.LastFetchedUtc);
        Assert.Equal(lastFetchedUtc.AddMinutes(5), result.NextAllowedFetchUtc);
        Assert.Equal(0, fakeEggApiClient.CallCount);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId);
        Assert.Equal(lastFetchedUtc, snapshot.LastFetchedUtc);
        Assert.Equal("{\"ok\":true}", snapshot.RawJson);
    }

    [Fact]
    public async Task FetchAndSaveAsync_ReturnsNull_WhenUserMissing()
    {
        var ct = TestContext.Current.CancellationToken;
        fakeEggApiClient.Reset(BuildResponse("ei-999", boostsUsed: 1));

        var result = await snapshotService.FetchAndSaveAsync("missing-user", "ei-999", ct);

        Assert.Null(result);
        Assert.Equal(0, fakeEggApiClient.CallCount);
        Assert.Empty(ctx.UserEggSnapshots);
    }

    [Fact]
    public async Task FetchAndSaveAsync_ReturnsNull_WhenEiUserIdMissing()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("sara");
        fakeEggApiClient.Reset(BuildResponse("ei-ignored", boostsUsed: 1));

        var result = await snapshotService.FetchAndSaveAsync(userId, " ", ct);

        Assert.Null(result);
        Assert.Equal(0, fakeEggApiClient.CallCount);
        Assert.Empty(ctx.UserEggSnapshots);
    }

    private async Task<string> CreateUserAsync(string seed)
    {
        var created = await userService.CreateAsync(new CreateUserDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"{seed}@test.it",
            Password = "JegKanHuskeDenneKode123!"
        });

        return created.UserId;
    }

    private static EggIncFirstContactResponse BuildResponse(string eiUserId, ulong boostsUsed)
    {
        return new EggIncFirstContactResponse
        {
            EiUserId = eiUserId,
            Backup = new Backup
            {
                EiUserId = eiUserId,
                UserName = "TestName",
                Stats = new Backup.Types.Stats
                {
                    BoostsUsed = boostsUsed
                },
                Game = new Backup.Types.Game
                {
                    SoulEggsD = 1234D,
                    EggsOfProphecy = 7,
                    GoldenEggsEarned = 1000,
                    GoldenEggsSpent = 250,
                    UncliamedGoldenEggs = 10
                },
                Virtue = new Backup.Types.Virtue
                {
                    EovEarned = { 5 }
                },
                Artifacts = new Backup.Types.Artifacts
                {
                    CraftingXp = 12.5D
                }
            }
        };
    }
}
