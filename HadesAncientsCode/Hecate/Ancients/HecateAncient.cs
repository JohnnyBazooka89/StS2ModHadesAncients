using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Hecate.Ancients;

[Pool(typeof(AncientEventModel))]
public class HecateAncient : CustomAncientModel
{
    public override string CustomScenePath => "hecate.tscn".AncientImagePath(HadesAncient.Hecate);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Hecate);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Hecate);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Hecate);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Hecate);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> relics =
            [
                AncientOption<Death>(),
                AncientOption<Eternity>(),
                AncientOption<Night>(),
                AncientOption<Origination>(),
                AncientOption<Persistence>(),
                AncientOption<TheCentaur>(),
                AncientOption<TheFuries>(),
                AncientOption<TheHuntress>(),
                AncientOption<TheLovers>(),
                AncientOption<TheMessenger>(),
                AncientOption<TheMoon>(),
                AncientOption<TheSorceress>(),
                AncientOption<TheSwiftRunner>(),
                AncientOption<TheUnseen>(),
                AncientOption<TheWaywardSon>()
            ];

            return new OptionPools(
                MakePool(relics.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 1 && !HadesAncientsModConfig.DisableHecate;
    }
}