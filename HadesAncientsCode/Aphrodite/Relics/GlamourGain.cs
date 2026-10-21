using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Relics;

[Pool(typeof(EventRelicPool))]
public class GlamourGain() : HadesAncientsRelic(HadesAncient.Aphrodite), IAfterArtifactPowerModifiedPowerAmountReceived
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool UsedThisTurn { get; set; }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public async Task AfterArtifactPowerModifiedPowerAmountReceived(
        ArtifactPower artifactPower,
        PowerModel blockedPower)
    {
        if (UsedThisTurn ||
            blockedPower.Applier != Owner.Creature ||
            !Owner.Creature.CombatState!.Enemies.Contains(artifactPower.Owner))
            return;
        await ActivateEffectAsync();
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0M || applier != Owner.Creature || !Owner.Creature.CombatState!.Enemies.Contains(power.Owner) ||
            power.GetTypeForAmount(amount) != PowerType.Debuff || UsedThisTurn)
            return;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    private async Task ActivateEffectAsync()
    {
        Flash();
        UsedThisTurn = true;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        UsedThisTurn = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}