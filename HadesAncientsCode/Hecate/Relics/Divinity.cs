using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Divinity() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
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
        new(CombatsKey, 2M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 24;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        ++CombatsSeen;
        return Task.CompletedTask;
    }


    public override Task AfterModifyingCardRewardOptions()
    {
        if (Owner.Creature.IsDead)
            return Task.CompletedTask;
        Flash();
        return Task.CompletedTask;
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != Owner
            || !options.Flags.HasFlag(CardCreationFlags.IsCardReward)
            || CombatsSeen % DynamicVars[CombatsKey].IntValue == 1)
        {
            return false;
        }

        var rng = Owner.RunState.Rng.CombatCardGeneration;

        List<CardCreationResult> candidatesToUpgrade =
            cardRewards.Where(reward => reward.Card is { IsUpgraded: false, IsUpgradable: true }).ToList();
        if (candidatesToUpgrade.Capacity == 0)
        {
            return false;
        }

        rng.Shuffle(candidatesToUpgrade);

        CardModel upgradedCard = Owner.RunState.CloneCard(candidatesToUpgrade[0].Card);
        CardCmd.Upgrade(upgradedCard);
        candidatesToUpgrade[0].ModifyCard(upgradedCard, this);


        return true;
    }
}