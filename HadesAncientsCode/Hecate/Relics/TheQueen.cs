using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheQueen() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string CombatsKey = "Combats";
    private int _combatsSeen;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override int DisplayAmount => CombatsSeen % DynamicVars[CombatsKey].IntValue;

    public override bool ShowCounter => true;

    [SavedProperty]
    private int CombatsSeen
    {
        get => _combatsSeen;
        set
        {
            AssertMutable();
            _combatsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(CombatsKey, 3M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 20;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.Encounter.RoomType != RoomType.Monster)
            return Task.CompletedTask;
        ++CombatsSeen;
        if (CombatsSeen % DynamicVars[CombatsKey].IntValue == 0)
        {
            Flash();
            IEnumerable<CardPoolModel> pools = Owner.UnlockState.CharacterCardPools
                .Where(DifferentCardPool)
                .ToList().StableShuffle(Owner.RunState.Rng.Niche)
                .Take(3).ToList();

            room.AddExtraReward(Owner, new CardReward(new CardCreationOptions(pools,
                    CardCreationSource.Other, CardRarityOddsType.RegularEncounter)
                .WithFlags(CardCreationFlags.NoCardPoolModifications), 3, Owner));
        }

        return Task.CompletedTask;
    }

    private bool DifferentCardPool(CardPoolModel p) => p != Owner.Character.CardPool;
}