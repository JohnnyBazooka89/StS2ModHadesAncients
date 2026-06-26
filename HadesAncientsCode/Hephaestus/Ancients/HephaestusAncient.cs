using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hephaestus.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Ancients;

[Pool(typeof(AncientEventModel))]
public class HephaestusAncient : CustomAncientModel
{
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
            List<AncientOption> blastRelicsPool =
            [
                AncientOption<FurnaceBlast>(),
                AncientOption<GrandCaldera>(),
                AncientOption<VolcanicFlourish>(),
                AncientOption<VolcanicStrike>()
            ];

            List<AncientOption> forgeArmorAndBlockRelicsPool =
            [
                AncientOption<HeavyMetal>(),
                AncientOption<SecuritySystem>(),
                AncientOption<SmithyRush>(),
                AncientOption<TrustyShield>()
            ];

            List<AncientOption> otherRelicsPool =
            [
                AncientOption<PremiumService>(),
                AncientOption<UncannyFortitude>()
            ];

            return new OptionPools(
                MakePool(blastRelicsPool.ToArray()),
                MakePool(forgeArmorAndBlockRelicsPool.ToArray()),
                MakePool(otherRelicsPool.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 3 && !HadesAncientsModConfig.DisableHephaestus;
    }
}