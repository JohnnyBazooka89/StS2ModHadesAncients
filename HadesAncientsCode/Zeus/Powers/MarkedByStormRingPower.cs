using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Zeus.Powers;

public class MarkedByStormRingPower() : HadesAncientsPower(HadesAncient.Zeus)
{
    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public override bool IsVisibleInternal =>
        Applier == LocalContext.GetMe(RunManager.Instance?.DebugOnlyGetState())!.Creature;
}