using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Chaos.Relics.Types;
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

namespace HadesAncients.HadesAncientsCode.Chaos.Relics;

[Pool(typeof(EventRelicPool))]
public class TheFuries()
    : HadesAncientsRelic(HadesAncient.Chaos), IArcanaRelic, IModifyDamageMultiplicativeCompatibility
{
    private const string MoreDamagePercentKey = "MoreDamagePercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MoreDamagePercentKey, 40M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 6;
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

        bool doesntIntendToAttack = !target.Monster?.IntendsToAttack ?? false;
        return 1M + (doesntIntendToAttack ? DynamicVars[MoreDamagePercentKey].BaseValue / 100M : 0);
    }
}