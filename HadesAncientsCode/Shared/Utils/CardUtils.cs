using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Utils;

public class CardUtils
{
    public static List<CardModel> GetUpgradedOrEnchantedCards(Player player)
    {
        return PileType.Deck.GetPile(player).Cards.Where(IsUpgradedOrEnchanted).ToList();
    }

    public static Boolean IsUpgradedOrEnchanted(CardModel card)
    {
        return card.IsUpgraded || card.Enchantment != null;
    }
}