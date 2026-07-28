using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Dionysus.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Dionysus.Ancients;

[Pool(typeof(AncientEventModel))]
public class DionysusAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(650f, 110f);

    public override string CustomScenePath => "dionysus.tscn".AncientImagePath(HadesAncient.Dionysus);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Dionysus);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Dionysus);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Dionysus);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Dionysus);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> relics =
            [
                AncientOption<BottomlessDrink>(),
                AncientOption<BounceBack>(),
                AncientOption<DrunkenDash>(),
                AncientOption<DrunkenStupor>(),
                AncientOption<HappyHaze>(),
                AncientOption<HighTolerance>(),
                AncientOption<PersonalLoan>(),
                AncientOption<PremiumVintage>(),
                AncientOption<RecklessAbandon>(),
                AncientOption<StrongDrink>(),
                AncientOption<TipsyShot>(),
                AncientOption<WorryFree>(),
            ];

            return new OptionPools(
                MakePool(relics.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 3 && !HadesAncientsModConfig.DisableDionysus;
    }
}