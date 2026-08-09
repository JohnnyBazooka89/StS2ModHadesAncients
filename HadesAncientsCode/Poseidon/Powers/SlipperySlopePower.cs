using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HadesAncients.HadesAncientsCode.Poseidon.Powers;

public class SlipperySlopePower()
    : HadesAncientsTemporaryPower<FrothPower, StrengthPower>(HadesAncient.Poseidon)
{
    public override PowerType Type => PowerType.Debuff;

    protected override bool InvertInternalPowerAmount => true;

    public override LocString Description => new("powers", "TEMPORARY_STRENGTH_DOWN.description");

    protected override string SmartDescriptionLocKey => "TEMPORARY_STRENGTH_DOWN.smartDescription";
}