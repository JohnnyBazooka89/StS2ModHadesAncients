using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class VitalSign() : HadesAncientsRelic(HadesAncient.Artemis), IModifyDamageMultiplicativeCompatibility
{
    private const string HpThresholdKey = "HpThreshold";
    private const string MoreDamagePercentKey = "MoreDamagePercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HpThresholdKey, 50M),
        new(MoreDamagePercentKey, 50M)
    ];

    public decimal ModifyDamageMultiplicativeCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || (dealer != Owner.Creature && dealer != Owner.Osty) ||
            target == null)
            return 1M;

        bool isHealthy = target.CurrentHp >= target.MaxHp * (DynamicVars[HpThresholdKey].BaseValue / 100M);
        return 1M + (isHealthy ? DynamicVars[MoreDamagePercentKey].BaseValue / 100M : 0);
    }
}