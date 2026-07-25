using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Origination()
    : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic, IModifyDamageMultiplicativeCompatibility
{
    private const string MoreDamagePercentKey = "MoreDamagePercent";
    private const string DebuffsThresholdKey = "DebuffsThreshold";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MoreDamagePercentKey, 40M),
        new(DebuffsThresholdKey, 2M),
    ];

    public int GetArcanaRelicNumber()
    {
        return 14;
    }

    public Decimal ModifyDamageMultiplicativeCompatibility(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || (dealer != Owner.Creature && dealer != Owner.Osty) ||
            target == null)
            return 1M;

        int numberOfDebuffsOnMonster = target.Monster?.Creature.Powers
            .Where(power => power.Type == PowerType.Debuff)
            .Select(power => power.Id)
            .Distinct()
            .Count() ?? 0;
        return 1M + (numberOfDebuffsOnMonster >= DynamicVars[DebuffsThresholdKey].IntValue
            ? DynamicVars[MoreDamagePercentKey].BaseValue / 100M
            : 0);
    }
}