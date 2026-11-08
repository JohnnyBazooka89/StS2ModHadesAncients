using BaseLib.Patches.Localization;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Powers;

public class ChillPower() : HadesAncientsPower(HadesAncient.Demeter), IAddDumbVariablesToPowerDescription
{
    private const decimal StacksToTriggerDefaultValue = 4M;
    private const decimal DamageToDealDefaultValue = 12M;
    private const decimal IceStruckToApplyDefaultValue = 1M;

    private const string StacksToTriggerKey = "StacksToTrigger";
    private const string DamageToDealKey = "DamageToDeal";
    private const string IceStruckToApplyKey = "IceStruckToApply";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => GetInternalData<Data>().StacksApplied;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(StacksToTriggerKey, StacksToTriggerDefaultValue),
        new(DamageToDealKey, DamageToDealDefaultValue),
        new(IceStruckToApplyKey, IceStruckToApplyDefaultValue)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromPowerWithPowerHoverTips<IceStruckPower>()
    ];

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add(
            HadesAncientsMainFile.ModId + nameof(ChillPower) + StacksToTriggerKey,
            StacksToTriggerDefaultValue
        );

        description.Add(
            HadesAncientsMainFile.ModId + nameof(ChillPower) + DamageToDealKey,
            DamageToDealDefaultValue
        );

        description.Add(
            HadesAncientsMainFile.ModId + nameof(ChillPower) + IceStruckToApplyKey,
            IceStruckToApplyDefaultValue
        );
    }

    public override object InitInternalData() => new Data();

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0M || power != this)
            return;

        Data data = GetInternalData<Data>();
        data.StacksApplied += (int)amount;
        while (data.StacksApplied >= DynamicVars[StacksToTriggerKey].IntValue)
        {
            Flash();
            await CreatureCmd.Damage(
                choiceContext,
                Owner,
                DynamicVars[DamageToDealKey].BaseValue,
                ValueProp.Unpowered,
                Applier!);
            await PowerCmd.Apply<IceStruckPower>(choiceContext, Owner, 1M, Applier, null);

            data.StacksApplied -= DynamicVars[StacksToTriggerKey].IntValue;
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