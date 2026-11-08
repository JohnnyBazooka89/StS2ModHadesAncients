using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Rewards;

public sealed class CardEnchantReward(
    Player player,
    EnchantmentModel enchantment,
    int cardsToEnchant = 1,
    int amount = 1)
    : CustomReward(player)
{
    [CustomEnum] public static RewardType HadesAncientsEnchant;

    public EnchantmentModel Enchantment { get; } = enchantment;
    public int CardsToEnchant { get; } = cardsToEnchant;
    public int Amount { get; } = amount;

    public override RewardType RewardType => HadesAncientsEnchant;

    public override LocString Description => new("gameplay_ui", "HADESANCIENTS-ENCHANT_REWARD");

    public override bool IsPopulated => true;

    private static string RewardIcon => "the_enchantress.png".RelicImagePath(HadesAncient.Hecate);

    public override string IconPath => RewardIcon;

    public override CreateRewardFromSave<CustomReward> DeserializeMethod => CreateFromSerializable;

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = HadesAncientsEnchant,
            GoldAmount = CardsToEnchant
        };
    }

    private static CardEnchantReward CreateFromSerializable(SerializableReward save, Player player)
    {
        return new CardEnchantReward(player, ModelDb.Enchantment<Steady>(), save.GoldAmount);
    }

    public override void MarkContentAsSeen()
    {
    }

    public override void Populate()
    {
    }

    public override async Task<bool> OnSelect()
    {
        return await DoUnsyncedEnchant(
            Player,
            Enchantment,
            CardsToEnchant,
            Amount);
    }

    private static async Task<bool> DoUnsyncedEnchant(
        Player player,
        EnchantmentModel enchantment,
        int cardsToEnchant,
        int amount)
    {
        if (!PileType.Deck.GetPile(player).Cards.Any(enchantment.CanEnchant))
            return false;

        CardSelectorPrefs prefs = new(CardSelectorPrefs.EnchantSelectionPrompt, cardsToEnchant)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };

        List<CardModel> cards = (await CardSelectCmd.FromDeckForEnchantment(
            player,
            enchantment,
            amount,
            prefs)).ToList();

        foreach (CardModel card in cards)
            CardCmd.Enchant(enchantment.ToMutable(), card, amount);

        return cards.Count > 0;
    }
}