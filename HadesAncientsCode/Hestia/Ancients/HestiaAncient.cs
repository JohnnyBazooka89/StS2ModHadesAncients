using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Hestia.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

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
            List<AncientOption> attackRelics =
            [
                AncientOption<CardioGain>(),
                AncientOption<FireAway>(),
                AncientOption<FlameStrike>(),
                AncientOption<SmolderRing>()
            ];

            List<AncientOption> otherRelics =
            [
                AncientOption<ControlledBurn>(),
                AncientOption<FlameFlourish>(),
                AncientOption<GlowingCoal>(),
                AncientOption<HeatRush>(),
                AncientOption<HighlyFlammable>(),
                AncientOption<HotPot>(),
                AncientOption<SlowCooker>()
            ];

            WeightedList<AncientOption> attackRelicsPool = MakePool(attackRelics.ToArray());
            WeightedList<AncientOption> otherRelicsPool = MakePool(otherRelics.ToArray());

            return new OptionPools(attackRelicsPool, otherRelicsPool, otherRelicsPool);
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisableHestia;
    }
}