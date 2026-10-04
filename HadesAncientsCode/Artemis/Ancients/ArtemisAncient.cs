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
            List<AncientOption> zeroEnergyCardsRelicsPool =
            [
                AncientOption<EasyShot>(),
                AncientOption<FullyLoaded>(),
                AncientOption<HuntersInstinct>(),
                AncientOption<ShadowPounce>(),
            ];

            List<AncientOption> attackDamageIncreaseRelicsPool =
            [
                AncientOption<HuntersMark>(),
                AncientOption<PressurePoints>(),
                AncientOption<VitalSign>(),
                AncientOption<WhisperedPrayer>()
            ];

            List<AncientOption> otherRelicsPool =
            [
                AncientOption<HunterDash>(),
                AncientOption<KillingStroke>(),
                AncientOption<LethalSnare>(),
                AncientOption<SupportFire>()
            ];

            return new OptionPools(
                MakePool(zeroEnergyCardsRelicsPool.ToArray()),
                MakePool(attackDamageIncreaseRelicsPool.ToArray()),
                MakePool(otherRelicsPool.ToArray())
            );
        }
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 && !HadesAncientsModConfig.DisableArtemis;
    }
}