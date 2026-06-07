using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Zeus.Relics;

[Pool(typeof(EventRelicPool))]
public class AirQuality() : HadesAncientsRelic(HadesAncient.Zeus)
{
    private const string DamageMinimumKey = "DamageMinimum";
    private const string DamageThresholdKey = "DamageThreshold";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(DamageMinimumKey, 10M),
        new(DamageThresholdKey, 9M)
    ];

    public override Decimal ModifyHpLostAfterOstyLate(
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return (dealer != Owner.Creature && dealer != Owner.Osty) || target == Owner.Creature ||
               !props.IsPoweredAttack() || amount < 1M ||
               amount >= DynamicVars[DamageMinimumKey].BaseValue
            ? amount
            : DynamicVars[DamageMinimumKey].BaseValue;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }
}