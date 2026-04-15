using Bogus;
using Ei;

namespace Test.Builders;

public static class EggSnapshotServiceTestData
{
    private static readonly Faker<SnapshotPayload> PayloadFaker = new Faker<SnapshotPayload>()
        .RuleFor(x => x.UserName, f => f.Internet.UserName())
        .RuleFor(x => x.SoulEggs, f => f.Random.Double(500D, 5_000D))
        .RuleFor(x => x.EggsOfProphecy, f => (ulong)f.Random.Int(1, 20))
        .RuleFor(x => x.GoldenEggsEarned, f => (ulong)f.Random.Int(500, 2_000))
        .RuleFor(x => x.GoldenEggsSpent, f => (ulong)f.Random.Int(10, 500))
        .RuleFor(x => x.TruthEgg, f => (ulong)f.Random.Int(1, 10))
        .RuleFor(x => x.CraftingXp, f => f.Random.Double(1D, 50D));

    public static EggIncFirstContactResponse FirstContactResponse(
        string eiUserId,
        ulong boostsUsed,
        string? responseEiUserId = null,
        int seed = 501)
    {
        var payload = PayloadFaker.UseSeed(seed).Generate();

        return new EggIncFirstContactResponse
        {
            EiUserId = responseEiUserId ?? eiUserId,
            Backup = new Backup
            {
                EiUserId = eiUserId,
                UserName = payload.UserName,
                Stats = new Backup.Types.Stats
                {
                    BoostsUsed = boostsUsed
                },
                Game = new Backup.Types.Game
                {
                    SoulEggsD = payload.SoulEggs,
                    EggsOfProphecy = payload.EggsOfProphecy,
                    GoldenEggsEarned = payload.GoldenEggsEarned,
                    GoldenEggsSpent = payload.GoldenEggsSpent
                },
                Virtue = new Backup.Types.Virtue
                {
                    EovEarned = { (uint)payload.TruthEgg }
                },
                Artifacts = new Backup.Types.Artifacts
                {
                    CraftingXp = payload.CraftingXp
                }
            }
        };
    }

    private sealed class SnapshotPayload
    {
        public string UserName { get; set; } = string.Empty;
        public double SoulEggs { get; set; }
        public ulong EggsOfProphecy { get; set; }
        public ulong GoldenEggsEarned { get; set; }
        public ulong GoldenEggsSpent { get; set; }
        public ulong TruthEgg { get; set; }
        public double CraftingXp { get; set; }
    }
}


