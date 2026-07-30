using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
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

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheHuntress()
    : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic, IModifyDamageAdditiveCompatibility
{
    private const string MoreDamageKey = "MoreDamage";
    private const string MoreBlockKey = "MoreBlock";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MoreDamageKey, 3),
        new(MoreBlockKey, 3),
    ];

    public int GetArcanaRelicNumber()
    {
        return 3;
    }

    public Decimal ModifyDamageAdditiveCompatibility(Creature? target, Decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource is not { Type: CardType.Attack } ||
            (dealer != Owner.Creature && dealer != Owner.Osty))
            return 0M;

        return Owner.PlayerCombatState!.Energy == 0 ? DynamicVars[MoreDamageKey].IntValue : 0;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredCardOrMonsterMoveBlock() || cardSource is not { Type: CardType.Skill } ||
            (target != Owner.Creature && target != Owner.Osty))
            return 0M;

        return Owner.PlayerCombatState!.Energy == 0 ? DynamicVars[MoreBlockKey].IntValue : 0;
    }
}