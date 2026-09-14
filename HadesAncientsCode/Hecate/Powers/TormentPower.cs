using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hecate.Powers;

public class TormentPower() : HadesAncientsPower(HadesAncient.Hecate), IModifyDamageAdditiveCompatibility
{
    private const string AdditionalDamageKey = "AdditionalDamage";

    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public override bool IsVisibleInternal =>
        Applier == LocalContext.GetMe(RunManager.Instance?.DebugOnlyGetState())!.Creature;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(AdditionalDamageKey, 2M)
    ];

    public decimal ModifyDamageAdditiveCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || Owner != target || Applier != dealer)
        {
            return 0;
        }

        return DynamicVars[AdditionalDamageKey].BaseValue;
    }
}