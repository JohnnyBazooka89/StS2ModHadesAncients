using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Ares.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Ares.Ancients;

[Pool(typeof(AncientEventModel))]
public class AresAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(650f, 110f);

    public override string CustomScenePath => "ares.tscn".AncientImagePath(HadesAncient.Ares);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Ares);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Ares);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Ares);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Ares);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> otherRelics =
            [
                AncientOption<GrievousBlow>(),
                AncientOption<GrislyGain>(),
                AncientOption<MeatGrinder>(),
                AncientOption<MutualDestruction>(),
                AncientOption<ProfuseBleeding>(),
                AncientOption<RallyingCry>(),
                AncientOption<ViciousFlourish>(),
                AncientOption<ViciousStrike>(),
            ];

            List<AncientOption> sacrificialRelics =
            [
                AncientOption<BloodSpree>(),
                AncientOption<SanguinarySavor>(),
                AncientOption<StabbingRush>(),
                AncientOption<VisceralImpact>()
            ];

            return new OptionPools(
                MakePool(otherRelics.ToArray()),
                MakePool(sacrificialRelics.ToArray())
            );

            /*return new OptionPools(
                MakePool(AncientOption<ViciousStrike>()),
                MakePool(AncientOption<RallyingCry>()),
                MakePool(AncientOption<VisceralImpact>())
            );*/
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 3 && !HadesAncientsModConfig.DisableAres;
    }
}