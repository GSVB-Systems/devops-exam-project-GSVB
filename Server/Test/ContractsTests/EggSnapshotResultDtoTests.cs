using DevOpsAppContracts.Models;

namespace Test.ContractsTests;

public class EggSnapshotResultDtoTests
{
    [Fact]
    public void NewDto_HasExpectedDefaults()
    {
        var dto = new EggSnapshotResultDto();

        Assert.Null(dto.UserName);
        Assert.Null(dto.SoulEggs);
        Assert.Null(dto.EggsOfProphecy);
        Assert.Null(dto.TruthEggs);
        Assert.Null(dto.GoldenEggsBalance);
        Assert.Null(dto.Mer);
        Assert.Null(dto.Jer);
        Assert.Null(dto.Eb);
        Assert.Equal(default, dto.LastFetchedUtc);
        Assert.Equal(default, dto.NextAllowedFetchUtc);
        Assert.False(dto.WasFetched);
    }

    [Fact]
    public void Dto_AssignedValues_ArePreserved()
    {
        var lastFetchedUtc = DateTime.UtcNow;
        var nextAllowedFetchUtc = lastFetchedUtc.AddMinutes(5);

        var dto = new EggSnapshotResultDto
        {
            UserName = "EggPlayer",
            SoulEggs = 1234.5,
            EggsOfProphecy = 10,
            TruthEggs = 4,
            GoldenEggsBalance = 4200,
            Mer = 12.34,
            Jer = 56.78,
            Eb = 90.12,
            LastFetchedUtc = lastFetchedUtc,
            NextAllowedFetchUtc = nextAllowedFetchUtc,
            WasFetched = true
        };

        Assert.Equal("EggPlayer", dto.UserName);
        Assert.Equal(1234.5, dto.SoulEggs);
        Assert.Equal((ulong)10, dto.EggsOfProphecy);
        Assert.Equal((ulong)4, dto.TruthEggs);
        Assert.Equal(4200, dto.GoldenEggsBalance);
        Assert.Equal(12.34, dto.Mer);
        Assert.Equal(56.78, dto.Jer);
        Assert.Equal(90.12, dto.Eb);
        Assert.Equal(lastFetchedUtc, dto.LastFetchedUtc);
        Assert.Equal(nextAllowedFetchUtc, dto.NextAllowedFetchUtc);
        Assert.True(dto.WasFetched);
    }
}

