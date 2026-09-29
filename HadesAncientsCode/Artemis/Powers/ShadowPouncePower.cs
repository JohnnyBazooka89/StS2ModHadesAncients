using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Artemis.Powers;

public class ShadowPouncePower() : HadesAncientsPower(HadesAncient.Artemis)
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (!ShouldModifyCost(card))
            return false;

        modifiedCost = 0m;
        return true;
    }

    public override bool TryModifyStarCost(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldModifyCost(card))
            return false;

        modifiedCost = 0M;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!ShouldModifyCost(cardPlay.Card) || cardPlay.IsAutoPlay)
            return;

        await PowerCmd.Decrement(this);
    }

    private bool ShouldModifyCost(CardModel card)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Skill)
            return false;

        return card.Pile?.Type is PileType.Hand or PileType.Play;
    }
}