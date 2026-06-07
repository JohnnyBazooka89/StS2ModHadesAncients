using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Athena.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HadesAncients.HadesAncientsCode.Athena.Ancients;

[Pool(typeof(AncientEventModel))]
public class AthenaAncient : CustomAncientModel
{
    public override string CustomScenePath => "athena.tscn".AncientImagePath(HadesAncient.Athena);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Athena);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Athena);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Athena);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Athena);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> relics =
            [
                AncientOption<BrilliantRiposte>(),
                AncientOption<Vajra>(),
                AncientOption<OddlySmoothStone>()
            ];

            return new OptionPools(
                MakePool(relics.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisableAthena;
    }
}