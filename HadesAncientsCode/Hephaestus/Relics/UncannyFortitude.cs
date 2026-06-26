using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hephaestus.SpireFields;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using HadesAncients.HadesAncientsCode.Shared.Vars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class UncannyFortitude() : HadesAncientsRelic(HadesAncient.Hephaestus), IAfterCardUpgrade, IAfterLoadRun
{
    private const string TotalHpToGainKey = "TotalHpToGain";

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(3M),
        new(TotalHpToGainKey + "Base", 0M),
        new(TotalHpToGainKey + "Extra", 1M),
        new OutsideCombatCalculatedVar(TotalHpToGainKey).WithMultiplier(static (relic, _) =>
            CardUtils.GetUpgradedOrEnchantedCards(relic.Owner).Count * relic.DynamicVars.MaxHp.BaseValue)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public async Task AfterCardUpgrade(CardModel card)
    {
        await AddMaxHpIfNotAddedAlready(card);
    }

    public Task AfterLoadRun(SerializableRoom? room)
    {
        foreach (CardModel card in CardUtils.GetUpgradedOrEnchantedCards(Owner))
        {
            HephaestusSpireFields.UncannyFortitudeUpgradedOrEnchantedCards.Set(card, true);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? clonedBy)
    {
        await AddMaxHpIfNotAddedAlready(card);
    }

    private async Task AddMaxHpIfNotAddedAlready(CardModel card)
    {
        if (card.Owner != Owner || card.Pile?.Type != PileType.Deck ||
            !CardUtils.IsUpgradedOrEnchanted(card) ||
            HephaestusSpireFields.UncannyFortitudeUpgradedOrEnchantedCards.Get(card))
        {
            return;
        }

        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        HephaestusSpireFields.UncannyFortitudeUpgradedOrEnchantedCards.Set(card, true);
    }

    public override async Task AfterObtained()
    {
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature,
            ((OutsideCombatCalculatedVar)DynamicVars[TotalHpToGainKey]).CalculateCustom(null));
        foreach (CardModel card in CardUtils.GetUpgradedOrEnchantedCards(Owner))
        {
            HephaestusSpireFields.UncannyFortitudeUpgradedOrEnchantedCards.Set(card, true);
        }
    }
}