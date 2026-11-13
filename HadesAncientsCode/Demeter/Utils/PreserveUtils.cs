using HadesAncients.HadesAncientsCode.Demeter.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace HadesAncients.HadesAncientsCode.Demeter.Utils;

public static class PreserveUtils
{
    public static async Task<bool> SelectAndEnchant(Player player, int cardsToPreserve, bool cancelable)
    {
        EnchantmentModel enchantment = ModelDb.Enchantment<Icebound>();
        int selectableCards = PileType.Deck.GetPile(player).Cards.Count(enchantment.CanEnchant);
        if (selectableCards == 0)
            return false;

        int selectionCount = Math.Min(cardsToPreserve, selectableCards);
        CardSelectorPrefs prefs = new(CardSelectorPrefs.EnchantSelectionPrompt, selectionCount)
        {
            Cancelable = cancelable,
            RequireManualConfirmation = true
        };
        List<CardModel> cards = (await CardSelectCmd.FromDeckForEnchantment(
            player, enchantment, selectionCount, prefs)).ToList();

        foreach (CardModel card in cards)
        {
            CardCmd.Enchant<Icebound>(card, 1M);
            NCardEnchantVfx? effect = NCardEnchantVfx.Create(card);
            if (effect != null)
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(effect);
        }

        return cards.Count > 0;
    }
}
