using Bogus;

namespace Test.Builders;

public sealed record EggSnapshotFormulaInput
{
    public double? SoulEggs { get; init; }
    public ulong? EggsOfProphecy { get; init; }
    public ulong? BoostsUsed { get; init; }
    public double? CraftingXp { get; init; }
    public long? GoldenEggsBalance { get; init; }
    public ulong? GoldenEggsSpent { get; init; }
    public ulong? TruthEggs { get; init; }
    public uint? SoulFoodLevels { get; init; }
    public uint? ProphecyBonusLevels { get; init; }
}

public static class EggSnapshotFormulasTestData
{
    private static readonly Faker<EggSnapshotFormulaInput> InputFaker = new Faker<EggSnapshotFormulaInput>()
        .RuleFor(x => x.SoulEggs, f => f.Random.Double(100D, 10_000_000_000_000_000_000D))
        .RuleFor(x => x.EggsOfProphecy, f => (ulong)f.Random.Int(1, 20))
        .RuleFor(x => x.BoostsUsed, f => (ulong)f.Random.Int(0, 200))
        .RuleFor(x => x.CraftingXp, f => f.Random.Double(0D, 100D))
        .RuleFor(x => x.GoldenEggsBalance, f => f.Random.Long(0, 1_000_000))
        .RuleFor(x => x.GoldenEggsSpent, f => (ulong)f.Random.Int(0, 500_000))
        .RuleFor(x => x.TruthEggs, f => (ulong)f.Random.Int(0, 20))
        .RuleFor(x => x.SoulFoodLevels, f => (uint)f.Random.Int(1, 200))
        .RuleFor(x => x.ProphecyBonusLevels, f => (uint)f.Random.Int(1, 100));

    public static EggSnapshotFormulaInput Create(int seed = 401) => InputFaker.UseSeed(seed).Generate();
}

