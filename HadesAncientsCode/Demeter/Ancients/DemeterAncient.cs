using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Demeter.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Demeter.Ancients;

[Pool(typeof(AncientEventModel))]
public class DemeterAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(600f, 110f);

    public override string CustomScenePath => "demeter.tscn".AncientImagePath(HadesAncient.Demeter);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Demeter);
    public override string CustomMapIconOutlinePath =>
        "map_icon_outline.png".AncientImagePath(HadesAncient.Demeter);
    public override string CustomRunHistoryIconPath =>
        "run_history_icon.png".AncientImagePath(HadesAncient.Demeter);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Demeter);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> relics =
            [
                AncientOption<ArcticGale>(),
                AncientOption<ArcticRing>(),
                AncientOption<ColdStorage>(),
                AncientOption<FrigidRush>(),
                AncientOption<FrostyVeneer>(),
                AncientOption<IceStrike>(),
                AncientOption<PlentifulForage>(),
                AncientOption<SnowQueen>(),
                AncientOption<SteadyGrowth>(),
                AncientOption<TranquilGain>(),
                AncientOption<WeedKiller>(),
                AncientOption<WinterHarvest>(),
            ];

            return new OptionPools(
                MakePool(relics.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 3 && !HadesAncientsModConfig.DisableDemeter;
    }
}