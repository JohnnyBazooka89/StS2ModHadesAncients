using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HadesAncients.HadesAncientsCode.Zeus.Powers;

public class MarkedByStormRingPower() : HadesAncientsPower(HadesAncient.Zeus)
{
    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;
}