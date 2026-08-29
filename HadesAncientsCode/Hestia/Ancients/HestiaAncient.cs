using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Hestia.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HadesAncients.HadesAncientsCode.Hestia.Ancients;

[Pool(typeof(AncientEventModel))]
public class HestiaAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(650f, 110f);

    public override string CustomScenePath => "hestia.tscn".AncientImagePath(HadesAncient.Hestia);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Hestia);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Hestia);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Hestia);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Hestia);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> relics =
            [
            ];

            return new OptionPools(
                MakePool(AncientOption<GlowingCoal>()),
                MakePool(AncientOption<OddlySmoothStone>()),
                MakePool(AncientOption<DataDisk>())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisableHestia;
    }
}