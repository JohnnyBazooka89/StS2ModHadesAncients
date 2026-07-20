using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Chaos.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Chaos.Ancients;

[Pool(typeof(AncientEventModel))]
public class ChaosAncient : CustomAncientModel
{
    public override string CustomScenePath => "chaos.tscn".AncientImagePath(HadesAncient.Chaos);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Chaos);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Chaos);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Chaos);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Chaos);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> relics =
            [
                AncientOption<Eternity>(),
                AncientOption<Persistence>(),
                AncientOption<TheFuries>(),
                AncientOption<TheHuntress>(),
                AncientOption<TheMessenger>(),
                AncientOption<TheMoon>(),
                AncientOption<TheSorceress>(),
                AncientOption<TheWaywardSon>(),
            ];

            return new OptionPools(
                MakePool(relics.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 1 && !HadesAncientsModConfig.DisableChaos;
    }
}