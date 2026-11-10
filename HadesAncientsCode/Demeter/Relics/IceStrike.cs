using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Demeter.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class IceStrike() : HadesAncientsRelic(HadesAncient.Demeter), IModifyDamageAdditiveCompatibility
{
    private const string ChillToApplyKey = "ChillToApply";
    private const string DamageIncreaseKey = "DamageIncrease";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(ChillToApplyKey, 1M),
        new(DamageIncreaseKey, 3M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromPowerWithPowerHoverTips<ChillPower>()
    ];

    public decimal ModifyDamageAdditiveCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if ((dealer != Owner.Creature && dealer != Owner.Osty) || !props.IsPoweredAttack())
            return 0M;

        return DynamicVars[DamageIncreaseKey].BaseValue;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if ((dealer != Owner.Creature && dealer != Owner.Osty) || !props.IsPoweredAttack())
            return;

        Flash();
        await PowerCmd.Apply<ChillPower>(choiceContext, target, DynamicVars[ChillToApplyKey].BaseValue, Owner.Creature,
            null);
    }
}