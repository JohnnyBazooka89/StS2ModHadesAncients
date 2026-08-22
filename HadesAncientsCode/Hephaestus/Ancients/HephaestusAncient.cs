using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Hephaestus.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Ancients;

[Pool(typeof(AncientEventModel))]
public class HephaestusAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(500f, 110f);

    public override string CustomScenePath => "hephaestus.tscn".AncientImagePath(HadesAncient.Hephaestus);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Hephaestus);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Hephaestus);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Hephaestus);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Hephaestus);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> offensiveRelicsPool =
            [
                AncientOption<FurnaceBlast>(),
                AncientOption<GrandCaldera>(),
                AncientOption<MartialArt>(),
                AncientOption<VolcanicStrike>()
            ];

            List<AncientOption> defensiveRelicsPool =
            [
                AncientOption<HeavyMetal>(),
                AncientOption<SecuritySystem>(),
                AncientOption<SmithyRush>(),
                AncientOption<TrustyShield>()
            ];

            List<AncientOption> otherRelicsPool =
            [
                AncientOption<PremiumService>(),
                AncientOption<ToughGain>(),
                AncientOption<UncannyFortitude>(),
                AncientOption<VolcanicFlourish>()
            ];

            return new OptionPools(
                MakePool(offensiveRelicsPool.ToArray()),
                MakePool(defensiveRelicsPool.ToArray()),
                MakePool(otherRelicsPool.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 3 && !HadesAncientsModConfig.DisableHephaestus;
    }
}