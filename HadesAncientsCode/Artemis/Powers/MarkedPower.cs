using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Artemis.Powers;

public class MarkedPower() : HadesAncientsPower(HadesAncient.Artemis), IModifyDamageMultiplicativeCompatibility
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal ModifyDamageMultiplicativeCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (Owner != target || !props.IsPoweredAttack())
            return 1M;
        Data internalData = GetInternalData<Data>();

        return internalData.CommandToModify != null && cardSource != null &&
               cardSource != internalData.CommandToModify.ModelSource
            ? 1M
            : 2M;
    }

    public override object InitInternalData() => new Data();

    public override Task BeforeAttack(AttackCommand command)
    {
        if (!command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        Data internalData = GetInternalData<Data>();
        if (internalData.CommandToModify != null ||
            command.ModelSource != null && command.ModelSource is not CardModel ||
            !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        internalData.CommandToModify = command;
        internalData.WasAttacked = false;
        return Task.CompletedTask;
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (Owner != target || result.TotalDamage <= 0)
        {
            return Task.CompletedTask;
        }

        Data internalData = GetInternalData<Data>();
        internalData.WasAttacked = true;
        return Task.CompletedTask;
    }

    public override async Task AfterAttack(
        PlayerChoiceContext choiceContext,
        AttackCommand command)
    {
        Data internalData = GetInternalData<Data>();

        if (command != internalData.CommandToModify)
            return;

        if (internalData.WasAttacked)
        {
            await PowerCmd.Decrement(this);
        }

        internalData.CommandToModify = null;
        internalData.WasAttacked = false;
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        if (Owner.IsAlive)
        {
            await PowerCmd.Remove(this);
        }
    }

    private class Data
    {
        public AttackCommand? CommandToModify;
        public bool WasAttacked;
    }
}