using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Demeter.Enchantments;

public class Icebound() : HadesAncientsEnchantment(HadesAncient.Demeter)
{
    public override bool HasExtraCardText => true;

    public override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Retain);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Card.Owner || combatState.RoundNumber != 1 || Card.Pile?.Type != PileType.Draw)
            return;

        await CardPileCmd.Add(Card, PileType.Hand);
    }

    public override Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        if (player == Card.Owner && retainedCards.Contains(Card))
            Card.EnergyCost.AddUntilPlayed(-1, reduceOnly: true);

        return Task.CompletedTask;
    }
}