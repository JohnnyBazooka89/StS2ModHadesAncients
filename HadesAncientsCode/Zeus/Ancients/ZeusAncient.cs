using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using HadesAncients.HadesAncientsCode.Zeus.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Zeus.Ancients;

[Pool(typeof(AncientEventModel))]
public class ZeusAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(650f, 110f);

    public override string CustomScenePath => "zeus.tscn".AncientImagePath(HadesAncient.Zeus);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Zeus);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Zeus);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Zeus);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Zeus);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> energyFocusedRelicsPool =
            [
                AncientOption<ElectricOverload>(),
                AncientOption<HeavenFlourish>(),
                AncientOption<IonicGain>(),
                AncientOption<StormRing>()
            ];

            List<AncientOption> buffAttacksRelicsPool =
            [
                AncientOption<AirQuality>(),
                AncientOption<DoubleStrike>(),
                AncientOption<HeavenStrike>(),
                AncientOption<StaticShock>()
            ];

            List<AncientOption> otherRelicsPool =
            [
                AncientOption<DivineVengeance>(),
                AncientOption<PowerSurge>(),
                AncientOption<ShockingLoss>(),
                AncientOption<ThunderRush>(),
            ];

            return new OptionPools(
                MakePool(energyFocusedRelicsPool.ToArray()),
                MakePool(buffAttacksRelicsPool.ToArray()),
                MakePool(otherRelicsPool.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisableZeus;
    }
}