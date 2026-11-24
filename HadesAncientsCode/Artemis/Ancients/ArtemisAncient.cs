using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Artemis.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Artemis.Ancients;

[Pool(typeof(AncientEventModel))]
public class ArtemisAncient : CustomAncientModel
{
    public Vector2 ChooseTheAncientPortalExtraOffset => new(650f, 30f);

    public override string CustomScenePath => "artemis.tscn".AncientImagePath(HadesAncient.Artemis);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Artemis);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Artemis);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Artemis);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Artemis);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            WeightedList<AncientOption> zeroEnergyCardsRelicsPool =
            [
                AncientOption<EasyShot>(),
                AncientOption<FullyLoaded>(),
                AncientOption<HunterInstinct>(),
                AncientOption<ShadowPounce>(),
            ];

            WeightedList<AncientOption> attackDamageIncreaseRelicsPool =
            [
                AncientOption<HuntersMark>(),
                AncientOption<PressurePoints>(),
                AncientOption<VitalSign>(),
                AncientOption<WhisperedPrayer>()
            ];

            WeightedList<AncientOption> otherRelicsPool =
            [
                AncientOption<HunterDash>(),
                AncientOption<KillingStroke>(),
                AncientOption<LethalSnare>(),
                AncientOption<SupportFire>()
            ];

            return new OptionPools(zeroEnergyCardsRelicsPool, attackDamageIncreaseRelicsPool, otherRelicsPool);
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisableArtemis;
    }
}