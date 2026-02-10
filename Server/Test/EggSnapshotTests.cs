using DevOpsAppContracts.Models;
using DevOpsAppRepo;
using DevOpsAppRepo.Entities;
using DevOpsAppService.Interfaces;
using Ei;
using System.Security.Cryptography;
using System.Text;

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
        Assert.Equal(HashEiUserId("ei-123"), result.EiUserIdHash);

        var snapshot = ctx.UserEggSnapshots.Single(s => s.UserId == userId);
        Assert.Equal(HashEiUserId("ei-123"), snapshot.EiUserIdHash);
        Assert.Equal((ulong)42, snapshot.BoostsUsed);
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
            EiUserIdHash = "hash",
            BoostsUsed = 1,
            LastFetchedUtc = lastFetchedUtc,
            RawJson = "{\"ok\":true}"
        });
        await ctx.SaveChangesAsync(ct);

        fakeEggApiClient.Reset(BuildResponse("ei-new", boostsUsed: 99));

        var result = await snapshotService.FetchAndSaveAsync(userId, "ei-new", ct);

        Assert.NotNull(result);
        Assert.False(result!.WasFetched);
        Assert.Equal("hash", result.EiUserIdHash);
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
                Stats = new Backup.Types.Stats
                {
                    BoostsUsed = boostsUsed
                }
            }
        };
    }

    private static string HashEiUserId(string eiUserId)
    {
        var bytes = Encoding.UTF8.GetBytes(eiUserId);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
