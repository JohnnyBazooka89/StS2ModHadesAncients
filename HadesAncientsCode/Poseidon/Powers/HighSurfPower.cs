using HadesAncients.HadesAncientsCode.Poseidon.Relics;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HadesAncients.HadesAncientsCode.Poseidon.Powers;

public class HighSurfPower()
    : HadesAncientsTemporaryPower<HighSurf, DexterityPower>(HadesAncient.Poseidon)
{
    public override LocString Description => new("powers", "TEMPORARY_DEXTERITY_POWER.description");
    protected override string SmartDescriptionLocKey => "TEMPORARY_DEXTERITY_POWER.smartDescription";
}