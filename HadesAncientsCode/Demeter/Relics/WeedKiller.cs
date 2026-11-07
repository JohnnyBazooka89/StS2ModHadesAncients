using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Demeter.Cards;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class WeedKiller() : HadesAncientsRelic(HadesAncient.Demeter)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromCardWithCardHoverTips<WeedOut>()
    ];

    public override async Task AfterObtained()
    {
        List<CardModel> deck = PileType.Deck.GetPile(Owner).Cards.ToList();
        int cardsToTransform = Math.Min(DynamicVars.Cards.IntValue, deck.Count);
        if (cardsToTransform == 0)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            cardsToTransform,
            cardsToTransform)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };
        List<CardTransformation> transformations = (await CardSelectCmd.FromDeckForTransformation(
                Owner,
                prefs,
                original => new CardTransformation(original, CreateWeedOutFromOriginal(original, true))))
            .Select(original => new CardTransformation(original, CreateWeedOutFromOriginal(original, false)))
            .ToList();
        if (transformations.Count == 0)
            return;

        await CardCmd.Transform(transformations, Owner.PlayerRng.Transformations);
    }

    private CardModel CreateWeedOutFromOriginal(CardModel original, bool forPreview)
    {
        CardModel card = forPreview
            ? ModelDb.Card<WeedOut>().ToMutable()
            : Owner.RunState.CreateCard<WeedOut>(Owner);

        if (original.IsUpgraded && card.IsUpgradable)
        {
            if (forPreview)
                card.UpgradeInternal();
            else
                CardCmd.Upgrade(card);
        }

        if (original.Enchantment == null)
            return card;

        EnchantmentModel enchantment = (EnchantmentModel)original.Enchantment.MutableClone();
        if (!enchantment.CanEnchant(card))
            return card;

        if (forPreview)
        {
            card.EnchantInternal(enchantment, enchantment.Amount);
            enchantment.ModifyCard();
        }
        else
        {
            CardCmd.Enchant(enchantment, card, enchantment.Amount);
        }

        return card;
    }
}