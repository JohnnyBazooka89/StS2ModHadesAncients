using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hecate.Powers;

public class InfatuatedPower() : HadesAncientsPower(HadesAncient.Hecate), IModifyDamageMultiplicativeCompatibility
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
        if (Owner != dealer || !props.IsPoweredAttack())
            return 1M;
        Data internalData = GetInternalData<Data>();
        return internalData.CommandToModify != null && internalData.CommandToModify.Attacker != dealer
            ? 1M
            : 0M;
    }

    public override object InitInternalData() => new Data();

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner || !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        Data internalData = GetInternalData<Data>();
        if (internalData.CommandToModify != null ||
            !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        internalData.CommandToModify = command;
        return Task.CompletedTask;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        Data internalData = GetInternalData<Data>();
        if (command != internalData.CommandToModify)
            return;
        internalData.CommandToModify = null;
        await PowerCmd.Decrement(this);
    }

    private class Data
    {
        public AttackCommand? CommandToModify;
    }
}