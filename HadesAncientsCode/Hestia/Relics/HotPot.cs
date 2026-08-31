using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class HotPot() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string HpLossReductionKey = "HpLossReduction";

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HpLossReductionKey, 3)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override Decimal ModifyHpLostAfterOsty(
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return target != Owner.Creature ? amount : Math.Max(0M, amount - DynamicVars[HpLossReductionKey].BaseValue);
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }
}