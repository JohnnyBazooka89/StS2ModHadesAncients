using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class BronzeSkin() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string LessDamagePercentKey = "LessDamagePercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(LessDamagePercentKey, 15M)
    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner.Creature || !props.IsPoweredAttack())
            return 1M;
        return 1 - DynamicVars[LessDamagePercentKey].BaseValue / 100M;
    }
}