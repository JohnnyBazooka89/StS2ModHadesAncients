using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HadesAncients.HadesAncientsCode.Zeus.Powers;

public class HeavenStruckPower() : HadesAncientsPower(HadesAncient.Zeus)
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
}