using HadesAncients.HadesAncientsCode.Poseidon.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Poseidon.Enchantments;

public class Splash() : HadesAncientsEnchantment(HadesAncient.Poseidon)
{
    public override bool HasExtraCardText => true;
    public override bool ShowAmount => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RupturePower>()
    ];

    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Attack;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (!props.IsPoweredAttack() || cardSource != Card)
            return;
        await PowerCmd.Apply<RupturePower>(choiceContext, target, Amount,
            Card.Owner.Creature, cardSource);
    }
}