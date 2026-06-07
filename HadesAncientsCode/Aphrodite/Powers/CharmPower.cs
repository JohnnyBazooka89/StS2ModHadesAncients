using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Powers;

public class CharmPower() : HadesAncientsPower(HadesAncient.Aphrodite)
{
    private const string StacksToStunKey = "StacksToStun";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => GetInternalData<Data>().StacksApplied;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(StacksToStunKey, 6M)
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
        if (data.StacksApplied >= DynamicVars[StacksToStunKey].IntValue)
        {
            Flash();
            await CreatureCmd.Stun(power.Owner);

            data.StacksApplied %= DynamicVars[StacksToStunKey].IntValue;
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