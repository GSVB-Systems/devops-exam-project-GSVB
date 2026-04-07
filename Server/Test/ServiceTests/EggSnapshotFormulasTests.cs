using DevOpsAppService.Services;
using Test.Builders;

namespace Test.ServiceTests;

public class EggSnapshotFormulasTests
{
    [Fact]
    public void Calculate_ReturnsNullMerAndJer_WhenSoulEggsMissingOrNonPositive()
    {
        var baseline = EggSnapshotFormulasTestData.Create(seed: 410);
        var missing = Calculate(baseline with { SoulEggs = null });
        var nonPositive = Calculate(baseline with { SoulEggs = 0D });

        Assert.Null(missing.Mer);
        Assert.Null(missing.Jer);
        Assert.Null(nonPositive.Mer);
        Assert.Null(nonPositive.Jer);
    }

    [Fact]
    public void Calculate_ReturnsNullJer_WhenEggsOfProphecyIsZero()
    {
        var input = EggSnapshotFormulasTestData.Create(seed: 411) with { EggsOfProphecy = 0UL };
        var result = Calculate(input);

        Assert.Null(result.Jer);
    }

    [Fact]
    public void Calculate_ReturnsNullEb_WhenProphecyLevelsMissingButEggsOfProphecyPositive()
    {
        var input = EggSnapshotFormulasTestData.Create(seed: 412) with
        {
            EggsOfProphecy = 7UL,
            ProphecyBonusLevels = null
        };
        var result = Calculate(input);

        Assert.Null(result.Eb);
    }

    [Fact]
    public void Calculate_ComputesEb_WhenEggsOfProphecyIsZero_WithoutProphecyLevels()
    {
        var input = EggSnapshotFormulasTestData.Create(seed: 413) with
        {
            SoulEggs = 100D,
            SoulFoodLevels = 3U,
            TruthEggs = 2UL,
            EggsOfProphecy = 0UL,
            ProphecyBonusLevels = null
        };
        var expectedEb = input.SoulEggs!.Value * (10D + input.SoulFoodLevels!.Value) * Math.Pow(1.01D, input.TruthEggs!.Value);
        var result = Calculate(input);

        Assert.NotNull(result.Eb);
        Assert.Equal(expectedEb, result.Eb!.Value, 10);
    }

    [Fact]
    public void Calculate_RoundsMerAndJer_ToTwoDecimals()
    {
        var input = EggSnapshotFormulasTestData.Create(seed: 414) with
        {
            SoulEggs = 1_234_567_890_123_456_000D,
            EggsOfProphecy = 7UL,
            TruthEggs = 1UL,
            SoulFoodLevels = 5U,
            ProphecyBonusLevels = 1U
        };
        var result = Calculate(input);

        Assert.NotNull(result.Mer);
        Assert.NotNull(result.Jer);
        Assert.Equal(Math.Round(result.Mer!.Value, 2), result.Mer.Value);
        Assert.Equal(Math.Round(result.Jer!.Value, 2), result.Jer.Value);
    }

    private static EggSnapshotCalculatedValues Calculate(EggSnapshotFormulaInput input)
    {
        return EggSnapshotFormulas.Calculate(
            soulEggs: input.SoulEggs,
            eggsOfProphecy: input.EggsOfProphecy,
            boostsUsed: input.BoostsUsed,
            craftingXp: input.CraftingXp,
            goldenEggsBalance: input.GoldenEggsBalance,
            goldenEggsSpent: input.GoldenEggsSpent,
            truthEggs: input.TruthEggs,
            soulFoodLevels: input.SoulFoodLevels,
            prophecyBonusLevels: input.ProphecyBonusLevels);
    }
}


