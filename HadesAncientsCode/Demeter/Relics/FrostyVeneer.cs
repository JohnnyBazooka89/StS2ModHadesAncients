using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class FrostyVeneer() : HadesAncientsRelic(HadesAncient.Demeter)
{
    public const string MaxHpLossKey = "MaxHpLoss";
    private decimal _damageReceivedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    private decimal DamageReceivedThisTurn
    {
        get => _damageReceivedThisTurn;
        set
        {
            AssertMutable();
            _damageReceivedThisTurn = value;
        }
    }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MaxHpLossKey, 10M)
    ];

    public override decimal ModifyHpLostAfterOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return !CombatManager.Instance.IsInProgress || target != Owner.Creature
            ? amount
            : Math.Min(amount, DynamicVars[MaxHpLossKey].BaseValue - DamageReceivedThisTurn);
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != Owner.Creature)
            return Task.CompletedTask;
        DamageReceivedThisTurn += result.UnblockedDamage;
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        DamageReceivedThisTurn = 0M;
        return Task.CompletedTask;
    }
}