using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
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

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class LethalSnare() : HadesAncientsRelic(HadesAncient.Artemis)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool UsedThisTurn { get; set; }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1),
        new PowerVar<VulnerablePower>(1),
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0M || applier != Owner.Creature || !Owner.Creature.CombatState!.Enemies.Contains(power.Owner) ||
            power.Type != PowerType.Debuff || UsedThisTurn)
            return;
        Flash();
        UsedThisTurn = true;
        await PowerCmd.Apply<WeakPower>(choiceContext, Owner.Creature.CombatState!.HittableEnemies,
            DynamicVars.Weak.BaseValue, Owner.Creature, null);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, Owner.Creature.CombatState!.HittableEnemies,
            DynamicVars.Vulnerable.BaseValue, Owner.Creature, null);
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