using DevOpsAppContracts.Models;
using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppService.Interfaces;
using DevOpsAppService.Services;
using Test.Builders;

namespace Test;

public class EggSnapshotTests(
    IEggSnapshotService snapshotService, IUserService userService, DevOpsAppDbContext ctx, FakeEggApiClient fakeEggApiClient)
{
    [Fact]
    public async Task FetchAndSaveAsync_ReturnsNull_WhenUserIdMissing()
    {
        var ct = TestContext.Current.CancellationToken;
        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-unused", boostsUsed: 0, seed: 510));

        var result = await snapshotService.FetchAndSaveAsync(" ", "ei-123", ct);

        Assert.Null(result);
        Assert.Equal(0, fakeEggApiClient.CallCount);
    }

    [Fact]
    public async Task FetchAndSaveAsync_CreatesSnapshotAndReturnsDto()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("lars");
        var response = EggSnapshotServiceTestData.FirstContactResponse("ei-123", boostsUsed: 42, seed: 511);
        fakeEggApiClient.Reset(response);

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-123", ct);

        Assert.NotNull(result);
        Assert.True(result!.WasFetched);
        Assert.Equal(result.LastFetchedUtc.AddMinutes(5), result.NextAllowedFetchUtc);
        Assert.Equal(response.Backup.UserName, result.UserName);
        Assert.Equal(response.Backup.Game.SoulEggsD, result.SoulEggs);
        Assert.Equal(response.Backup.Game.EggsOfProphecy, result.EggsOfProphecy);
        Assert.Equal((ulong)response.Backup.Virtue.EovEarned.Sum(x => (long)x), result.TruthEggs);
        Assert.Equal((long)response.Backup.Game.GoldenEggsEarned - (long)response.Backup.Game.GoldenEggsSpent, result.GoldenEggsBalance);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId);
        Assert.Equal("ei-123", snapshot.EiUserId);
        Assert.Equal(response.Backup.UserName, snapshot.UserName);
        Assert.Equal((ulong)42, snapshot.BoostsUsed);
        Assert.Equal(response.Backup.Game.SoulEggsD, snapshot.SoulEggs);
        Assert.Equal(response.Backup.Game.EggsOfProphecy, snapshot.EggsOfProphecy);
        Assert.Equal((ulong)response.Backup.Virtue.EovEarned.Sum(x => (long)x), snapshot.TruthEggs);
        Assert.Equal(response.Backup.Game.GoldenEggsEarned, snapshot.GoldenEggsEarned);
        Assert.Equal(response.Backup.Game.GoldenEggsSpent, snapshot.GoldenEggsSpent);
        Assert.Equal((long)response.Backup.Game.GoldenEggsEarned - (long)response.Backup.Game.GoldenEggsSpent, snapshot.GoldenEggsBalance);
        Assert.Equal(response.Backup.Artifacts.CraftingXp, snapshot.CraftingXp);
        Assert.False(string.IsNullOrWhiteSpace(snapshot.RawJson));
    }

    [Fact]
    public async Task FetchAndSaveAsync_WhenWithinMinInterval_ReturnsCached()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("ida");
        var lastFetchedUtc = DateTime.UtcNow;
        var calculated = EggSnapshotFormulas.Calculate(
            soulEggs: 55,
            eggsOfProphecy: 2,
            boostsUsed: 1,
            craftingXp: 2.5,
            goldenEggsBalance: 75,
            goldenEggsSpent: 25,
            truthEggs: 3,
            soulFoodLevels: null,
            prophecyBonusLevels: null);

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
            Mer = calculated.Mer,
            Jer = calculated.Jer,
            CraftingXp = 2.5,
            LastFetchedUtc = lastFetchedUtc,
              RawJson = "{\"ok\":true}",
              Id = Guid.NewGuid().ToString()
        });
        await ctx.SaveChangesAsync(ct);
                ctx.ChangeTracker.Clear();

        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-new", boostsUsed: 99, seed: 512));

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-previous", ct);

        Assert.NotNull(result);
        Assert.False(result!.WasFetched);
        Assert.Equal("CachedUser", result.UserName);
        Assert.Equal(55D, result.SoulEggs);
        Assert.Equal((ulong)2, result.EggsOfProphecy);
        Assert.Equal((ulong)3, result.TruthEggs);
        Assert.Equal(75, result.GoldenEggsBalance);
        var expectedLastFetchedUtc = DateTime.SpecifyKind(lastFetchedUtc, DateTimeKind.Unspecified);
        var actualLastFetchedUtc = DateTime.SpecifyKind(result.LastFetchedUtc, DateTimeKind.Unspecified);
        Assert.Equal(expectedLastFetchedUtc, actualLastFetchedUtc, TimeSpan.FromSeconds(1));
        Assert.Equal(expectedLastFetchedUtc.AddMinutes(5),
            DateTime.SpecifyKind(result.NextAllowedFetchUtc, DateTimeKind.Unspecified),
            TimeSpan.FromSeconds(1));
        Assert.Equal(0, fakeEggApiClient.CallCount);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId);
        var snapshotLastFetchedUtc = DateTime.SpecifyKind(snapshot.LastFetchedUtc, DateTimeKind.Unspecified);
        Assert.Equal(expectedLastFetchedUtc, snapshotLastFetchedUtc, TimeSpan.FromSeconds(1));
        Assert.Equal("{\"ok\": true}", snapshot.RawJson);
    }

    [Fact]
    public async Task FetchAndSaveAsync_WhenWithinMinInterval_RecalculatesAndPersistsMerJer()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("mona");
        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-unused", boostsUsed: 0, seed: 513));

        ctx.UserEggSnapshots.Add(new UserEggSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EiUserId = "ei-cache",
            UserName = "CacheRecalc",
            BoostsUsed = 1,
            SoulEggs = 1_000_000_000_000_000_000D,
            EggsOfProphecy = 1,
            TruthEggs = 0,
            GoldenEggsEarned = 20,
            GoldenEggsSpent = 10,
            GoldenEggsBalance = 10,
            LastFetchedUtc = DateTime.UtcNow,
            Mer = null,
            Jer = null,
            RawJson = "{}"
        });
        await ctx.SaveChangesAsync(ct);
        ctx.ChangeTracker.Clear();

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-cache", ct);

        Assert.NotNull(result);
        Assert.False(result!.WasFetched);
        Assert.NotNull(result.Mer);
        Assert.NotNull(result.Jer);
        Assert.Equal(0, fakeEggApiClient.CallCount);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId && s.EiUserId == "ei-cache");
        Assert.NotNull(snapshot.Mer);
        Assert.NotNull(snapshot.Jer);
    }

    [Fact]
    public async Task FetchAndSaveAsync_ReturnsNull_WhenUserMissing()
    {
        var ct = TestContext.Current.CancellationToken;
        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-999", boostsUsed: 1, seed: 514));

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
        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-ignored", boostsUsed: 1, seed: 515));

        var result = await snapshotService.FetchAndSaveAsync(userId, " ", ct);

        Assert.Null(result);
        Assert.Equal(0, fakeEggApiClient.CallCount);
        Assert.Empty(ctx.UserEggSnapshots);
    }

    [Fact]
    public async Task FetchAndSaveAsync_UsesBackupEiUserId_WhenResponseEiUserIdMissing()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("zara");
        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-backup", boostsUsed: 11, responseEiUserId: " ", seed: 516));

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-request", ct);

        Assert.NotNull(result);
        Assert.True(result!.WasFetched);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId);
        Assert.Equal("ei-backup", snapshot.EiUserId);
    }

    [Fact]
    public async Task FetchAndSaveAsync_WhenExistingOutsideInterval_NormalizesStatusAndUpdatesEiUserId()
    {
        var ct = TestContext.Current.CancellationToken;
        var userId = await CreateUserAsync("jon");
        var existing = new UserEggSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EiUserId = "ei-request",
            Status = " ",
            LastFetchedUtc = DateTime.UtcNow.AddMinutes(-10),
            RawJson = "{}"
        };
        ctx.UserEggSnapshots.Add(existing);
        await ctx.SaveChangesAsync(ct);
        ctx.ChangeTracker.Clear();

        fakeEggApiClient.Reset(EggSnapshotServiceTestData.FirstContactResponse("ei-backup-2", boostsUsed: 12, responseEiUserId: " ", seed: 517));

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-request", ct);

        Assert.NotNull(result);
        Assert.True(result!.WasFetched);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.Id == existing.Id);
        Assert.Equal("Alt", snapshot.Status);
        Assert.Equal("ei-backup-2", snapshot.EiUserId);
    }

    private async Task<string> CreateUserAsync(string seed)
    {
        var created = await userService.CreateAsync(new CreateUserDto
        {
            Username = "test-user",
            DiscordUsername = "test-discord",
            Email = $"{seed}@test.it",
            Password = "JegKanHuskeDenneKode123!"
        });

        return created.UserId;
    }

    // test response generation moved to Test.Builders.EggSnapshotServiceTestData
}