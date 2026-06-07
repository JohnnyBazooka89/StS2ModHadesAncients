using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Poseidon.Powers;

public class FrothPower() : HadesAncientsPower(HadesAncient.Poseidon)
{
    private const string StacksToTakeDamageKey = "StacksToTakeDamage";
    private const string DamageToTakeKey = "DamageToTake";
    private const string TemporaryStrengthToLoseKey = "TemporaryStrengthToLose";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => GetInternalData<Data>().StacksApplied;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(StacksToTakeDamageKey, 3M),
        new(DamageToTakeKey, 10M),
        new(TemporaryStrengthToLoseKey, 2M)
    ];

    public override object InitInternalData() => new Data();

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0M || power != this)
            return;

        Data data = GetInternalData<Data>();
        data.StacksApplied += (int)amount;
        while (data.StacksApplied >= DynamicVars[StacksToTakeDamageKey].IntValue)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, Owner, DynamicVars[DamageToTakeKey].BaseValue, ValueProp.Unpowered,
                Owner);
            await PowerCmd.Apply<SlipperySlopePower>(choiceContext, Owner,
                DynamicVars[TemporaryStrengthToLoseKey].BaseValue, Owner, null);

            data.StacksApplied -= DynamicVars[StacksToTakeDamageKey].IntValue;
            if (data.StacksApplied == 0)
            {
                await PowerCmd.Remove(this);
            }
        }

        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int StacksApplied;
    }
}