using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Utils;

public static class CardUtils
{
    public static List<CardModel> GetUpgradedOrEnchantedCards(Player player)
    {
        return PileType.Deck.GetPile(player).Cards.Where(IsUpgradedOrEnchanted).ToList();
    }

    public static bool IsUpgradedOrEnchanted(this CardModel card)
    {
        return card.IsUpgraded || card.Enchantment != null;
    }
    
    public static bool IsZeroEnergyCard(this CardModel card)
    {
        return card.EnergyCost.GetWithModifiers(CostModifiers.All) == 0 && !card.EnergyCost.CostsX;
    }
}