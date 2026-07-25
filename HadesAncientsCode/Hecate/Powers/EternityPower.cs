using HadesAncients.HadesAncientsCode.Hecate.Relics;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HadesAncients.HadesAncientsCode.Hecate.Powers;

public class EternityPower()
    : HadesAncientsTemporaryPower<Eternity, StrengthPower>(HadesAncient.Hecate)
{
    public override PowerType Type => PowerType.Debuff;

    protected override bool InvertInternalPowerAmount => true;

    public override LocString Description => new("powers", "TEMPORARY_STRENGTH_DOWN.description");

    protected override string SmartDescriptionLocKey => "TEMPORARY_STRENGTH_DOWN.smartDescription";
}