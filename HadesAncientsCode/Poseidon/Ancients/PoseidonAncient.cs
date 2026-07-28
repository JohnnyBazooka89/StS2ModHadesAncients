using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Poseidon.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Poseidon.Ancients;

[Pool(typeof(AncientEventModel))]
public class PoseidonAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(650f, 110f);

    public override string CustomScenePath => "poseidon.tscn".AncientImagePath(HadesAncient.Poseidon);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Poseidon);
    public override string CustomMapIconOutlinePath =>
        "map_icon_outline.png".AncientImagePath(HadesAncient.Poseidon);
    public override string CustomRunHistoryIconPath =>
        "run_history_icon.png".AncientImagePath(HadesAncient.Poseidon);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Poseidon);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> energyFocusedRelicsPool =
            [
                AncientOption<FloodGain>(),
                AncientOption<SecondWave>(),
                AncientOption<WaveFlourish>(),
                AncientOption<WaveStrike>(),
            ];

            List<AncientOption> buffAttacksRelicsPool =
            [
                AncientOption<HydraulicMight>(),
                AncientOption<KingTide>(),
                AncientOption<RazorShoals>(),
                AncientOption<SlipperySlope>(),
            ];

            List<AncientOption> otherRelicsPool =
            [
                AncientOption<BuriedTreasure>(),
                AncientOption<HighSurf>(),
                AncientOption<SeaStar>(),
                AncientOption<WaterFitness>(),
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
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisablePoseidon;
    }
}