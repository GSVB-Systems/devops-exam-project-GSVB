namespace DevOpsAppService.Services;

public sealed record EggSnapshotCalculatedValues(double? Mer, double? Jer, double? Cer, double? Eb);

public static class EggSnapshotFormulas
{
    public static EggSnapshotCalculatedValues Calculate(
        double? soulEggs,
        ulong? eggsOfProphecy,
        ulong? boostsUsed,
        double? craftingXp,
        long? goldenEggsBalance,
        ulong? goldenEggsSpent,
        ulong? truthEggs,
        uint? soulFoodLevels,
        uint? prophecyBonusLevels)
    {
        var pe = eggsOfProphecy.HasValue ? (double?)eggsOfProphecy.Value : null;
        var te = truthEggs.HasValue ? (double?)truthEggs.Value : null;

        var mer = CalculateMer(soulEggs, pe);
        var jer = CalculateJer(soulEggs, pe);
        var eb = CalculateEb(soulEggs, pe, te, soulFoodLevels, prophecyBonusLevels);

        return new EggSnapshotCalculatedValues(
            Mer: Round2(mer),
            Jer: Round2(jer),
            Cer: null,
            Eb: eb);
    }

    private static double? Round2(double? value)
    {
        return value.HasValue ? Math.Round(value.Value, 2) : null;
    }

    private static double? CalculateMer(double? soulEggs, double? eggsOfProphecy)
    {
        if (!soulEggs.HasValue || !eggsOfProphecy.HasValue)
        {
            return null;
        }

        if (soulEggs.Value <= 0)
        {
            return null;
        }

        var seQ = soulEggs.Value / 1_000_000_000_000_000_000D;
        if (seQ <= 0)
        {
            return null;
        }

        var logSe = Math.Log10(seQ);
        return (91D * logSe + 200D - eggsOfProphecy.Value) / 10D;
    }

    private static double? CalculateJer(double? soulEggs, double? eggsOfProphecy)
    {
        if (!soulEggs.HasValue || !eggsOfProphecy.HasValue || eggsOfProphecy.Value <= 0)
        {
            return null;
        }

        if (soulEggs.Value <= 0)
        {
            return null;
        }

        var logSe = Math.Log10(soulEggs.Value);
        var numerator = 0.1519D * Math.Pow(logSe, 3)
                        - 4.8517D * Math.Pow(logSe, 2)
                        + 48.248D * logSe
                        - 143.46D;

        var pe = eggsOfProphecy.Value;
        return (((numerator / pe) * 100D * pe + 100D * 49D) / (pe + 100D));
    }

    private static double? CalculateEb(
        double? soulEggs,
        double? eggsOfProphecy,
        double? truthEggs,
        uint? soulFoodLevels,
        uint? prophecyBonusLevels)
    {
        if (!soulEggs.HasValue
            || !eggsOfProphecy.HasValue
            || !truthEggs.HasValue
            || !soulFoodLevels.HasValue)
        {
            return null;
        }

        var se = soulEggs.Value;
        var pe = eggsOfProphecy.Value;
        var te = truthEggs.Value;

        if (pe > 0 && !prophecyBonusLevels.HasValue)
        {
            return null;
        }

        var baseMult = 10D + soulFoodLevels.Value;
        var prophecyMult = pe <= 0
            ? 1D
            : Math.Pow(1.05D + 0.01D * prophecyBonusLevels.Value, pe);
        var truthMult = Math.Pow(1.01D, te);

        return se * baseMult * prophecyMult * truthMult;
    }
}
