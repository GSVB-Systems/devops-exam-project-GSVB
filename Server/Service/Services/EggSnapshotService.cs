using DevOpsAppContracts.Models;
using DevOpsAppRepo.Entities;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Interfaces;
using Google.Protobuf;
using System.Linq;

namespace DevOpsAppService.Services;

public class EggSnapshotService : IEggSnapshotService
{
    private static readonly TimeSpan MinFetchInterval = TimeSpan.FromMinutes(5);
    private readonly IEggApiClient _eggApiClient;
    private readonly IUserRepository _userRepository;
    private readonly IUserEggSnapshotRepository _snapshotRepository;

    public EggSnapshotService(
        IEggApiClient eggApiClient,
        IUserRepository userRepository,
        IUserEggSnapshotRepository snapshotRepository)
    {
        _eggApiClient = eggApiClient;
        _userRepository = userRepository;
        _snapshotRepository = snapshotRepository;
    }

    public async Task<EggAccountRefreshDto?> FetchAndSaveAsync(
        string userId,
        string eiUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;
        if (string.IsNullOrWhiteSpace(eiUserId))
            return null;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return null;

        var existing = await _snapshotRepository.GetByUserAndEiUserIdAsync(userId, eiUserId);
        var now = DateTime.UtcNow;

        if (existing is not null && now - existing.LastFetchedUtc < MinFetchInterval)
        {
            var recalculated = EggSnapshotFormulas.Calculate(
                existing.SoulEggs,
                existing.EggsOfProphecy ?? 0UL,
                existing.BoostsUsed,
                existing.CraftingXp,
                existing.GoldenEggsBalance,
                existing.GoldenEggsSpent,
                existing.TruthEggs ?? 0UL,
                null,
                null);
            var mer = recalculated.Mer ?? existing.Mer;
            var jer = recalculated.Jer ?? existing.Jer;
            if (mer != existing.Mer || jer != existing.Jer)
            {
                existing.Mer = mer;
                existing.Jer = jer;
                await _snapshotRepository.UpdateAsync(existing);
                await _snapshotRepository.SaveChangesAsync();
            }

            return new EggAccountRefreshDto
            {
                UserName = existing.UserName,
                SoulEggs = existing.SoulEggs,
                EggsOfProphecy = existing.EggsOfProphecy,
                TruthEggs = existing.TruthEggs,
                GoldenEggsBalance = existing.GoldenEggsBalance,
                Mer = mer,
                Jer = jer,
                Eb = existing.Eb,
                LastFetchedUtc = existing.LastFetchedUtc,
                NextAllowedFetchUtc = existing.LastFetchedUtc.Add(MinFetchInterval),
                WasFetched = false
            };
        }

        // Fetch new data from Egg API

        var response = await _eggApiClient.GetFirstContactAsync(eiUserId, cancellationToken: cancellationToken);
        var rawJson = JsonFormatter.Default.Format(response);
        var resolvedEiUserId = string.IsNullOrWhiteSpace(response.EiUserId)
            ? response.Backup?.EiUserId
            : response.EiUserId;
        if (string.IsNullOrWhiteSpace(resolvedEiUserId))
            resolvedEiUserId = eiUserId;

        if (string.IsNullOrWhiteSpace(resolvedEiUserId))
            return null;

        if (existing is null)
        {
            existing = await _snapshotRepository.GetByUserAndEiUserIdAsync(userId, resolvedEiUserId);
        }

        var backup = response.Backup;
        var stats = backup?.Stats;
        var game = backup?.Game;
        var artifacts = backup?.Artifacts;
        var virtue = backup?.Virtue;

        var userName = backup?.HasUserName == true ? backup.UserName : null;
        var boostsUsed = stats?.HasBoostsUsed == true
            ? stats.BoostsUsed
            : (ulong?)null;
        var soulEggs = game?.HasSoulEggsD == true
            ? game.SoulEggsD
            : (game?.HasSoulEggs == true ? game.SoulEggs : (double?)null);
        var eggsOfProphecy = game?.HasEggsOfProphecy == true
            ? game.EggsOfProphecy
            : (ulong?)null;
        ulong? truthEggs = null;
        if (virtue?.EovEarned is not null && virtue.EovEarned.Count > 0)
        {
            truthEggs = (ulong)virtue.EovEarned.Sum(value => (long)value);
        }
        eggsOfProphecy ??= 0UL;
        truthEggs ??= 0UL;
        var goldenEggsEarned = game?.HasGoldenEggsEarned == true
            ? game.GoldenEggsEarned
            : (ulong?)null;
        var goldenEggsSpent = game?.HasGoldenEggsSpent == true
            ? game.GoldenEggsSpent
            : (ulong?)null;
        long? goldenEggsBalance = null;
        if (goldenEggsEarned.HasValue || goldenEggsSpent.HasValue)
        {
            goldenEggsBalance = (long)(goldenEggsEarned ?? 0)
                                 - (long)(goldenEggsSpent ?? 0);
        }
        var craftingXp = artifacts?.HasCraftingXp == true
            ? artifacts.CraftingXp
            : (double?)null;
        var epicResearch = game?.EpicResearch;
        var soulFoodLevels = GetEpicResearchLevel(epicResearch, "soul_eggs");
        var prophecyBonusLevels = GetEpicResearchLevel(epicResearch, "prophecy_bonus");

        var calculated = EggSnapshotFormulas.Calculate(
            soulEggs,
            eggsOfProphecy,
            boostsUsed,
            craftingXp,
            goldenEggsBalance,
            goldenEggsSpent,
            truthEggs,
            soulFoodLevels,
            prophecyBonusLevels);

        if (existing is null)
        {
            var mainAccount = await _snapshotRepository.GetMainByUserIdAsync(userId);
            var status = mainAccount is null ? "Main" : "Alt";

            existing = new UserEggSnapshot
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EiUserId = resolvedEiUserId,
                Status = status,
                UserName = userName,
                BoostsUsed = boostsUsed,
                SoulEggs = soulEggs,
                EggsOfProphecy = eggsOfProphecy,
                TruthEggs = truthEggs,
                GoldenEggsEarned = goldenEggsEarned,
                GoldenEggsSpent = goldenEggsSpent,
                GoldenEggsBalance = goldenEggsBalance,
                CraftingXp = craftingXp,
                Mer = calculated.Mer,
                Jer = calculated.Jer,
                Cer = calculated.Cer,
                Eb = calculated.Eb,
                LastFetchedUtc = now,
                RawJson = rawJson
            };
            await _snapshotRepository.AddAsync(existing);
        }
        else
        {
            existing.EiUserId = resolvedEiUserId;
            if (string.IsNullOrWhiteSpace(existing.Status))
            {
                existing.Status = "Alt";
            }
            existing.UserName = userName;
            existing.BoostsUsed = boostsUsed;
            existing.SoulEggs = soulEggs;
            existing.EggsOfProphecy = eggsOfProphecy;
            existing.TruthEggs = truthEggs;
            existing.GoldenEggsEarned = goldenEggsEarned;
            existing.GoldenEggsSpent = goldenEggsSpent;
            existing.GoldenEggsBalance = goldenEggsBalance;
            existing.CraftingXp = craftingXp;
            existing.Mer = calculated.Mer;
            existing.Jer = calculated.Jer;
            existing.Cer = calculated.Cer;
            existing.Eb = calculated.Eb;
            existing.LastFetchedUtc = now;
            existing.RawJson = rawJson;
            await _snapshotRepository.UpdateAsync(existing);
        }

        await _snapshotRepository.SaveChangesAsync();

        return new EggAccountRefreshDto
        {
            UserName = existing.UserName,
            SoulEggs = existing.SoulEggs,
            EggsOfProphecy = existing.EggsOfProphecy,
            TruthEggs = existing.TruthEggs,
            GoldenEggsBalance = existing.GoldenEggsBalance,
            Mer = existing.Mer,
            Jer = existing.Jer,
            Eb = existing.Eb,
            LastFetchedUtc = existing.LastFetchedUtc,
            NextAllowedFetchUtc = existing.LastFetchedUtc.Add(MinFetchInterval),
            WasFetched = true
        };
    }

    private static uint? GetEpicResearchLevel(
        global::Google.Protobuf.Collections.RepeatedField<global::Ei.Backup.Types.ResearchItem>? items,
        string researchId)
    {
        if (items is null || items.Count == 0)
        {
            return null;
        }

        foreach (var item in items)
        {
            if (string.Equals(item.Id, researchId, StringComparison.OrdinalIgnoreCase))
            {
                return item.Level;
            }
        }

        return null;
    }
}
