using HadesAncients.HadesAncientsCode.Ares.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Ares.Enchantments;

public class Wounding() : HadesAncientsEnchantment(HadesAncient.Ares)
{
    public override bool HasExtraCardText => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WoundsPower>()
    ];

    public override bool CanEnchant(CardModel c)
    {
        if (!base.CanEnchant(c))
            return false;
        return c.Type == CardType.Attack;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if ((dealer != Card.Owner.Creature && dealer != Card.Owner.Osty) || !props.IsPoweredAttack() ||
            result.TotalDamage <= 0 || cardSource != Card)
        {
            return;
        }

        await PowerCmd.Apply<WoundsPower>(choiceContext, target, result.TotalDamage / 2, Card.Owner.Creature, Card);
    }
}