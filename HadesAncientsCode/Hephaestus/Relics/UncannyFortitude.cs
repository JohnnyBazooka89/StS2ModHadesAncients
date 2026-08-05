using BaseLib.Utils;
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

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class UncannyFortitude()
    : HadesAncientsRelic(HadesAncient.Hephaestus), IAfterCardBecameUpgradedOrEnchanted
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

    public async Task AfterCardBecameUpgradedOrEnchanted(
        CardModel card)
    {
        if (card.Owner != Owner ||
            card.Pile?.Type != PileType.Deck)
        {
            return;
        }

        Flash();
        await CreatureCmd.GainMaxHp(
            Owner.Creature,
            DynamicVars.MaxHp.BaseValue
        );
    }

    public override async Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? clonedBy)
    {
        if (oldPileType == PileType.Deck ||
            card.Owner != Owner ||
            card.Pile?.Type != PileType.Deck ||
            !CardUtils.IsUpgradedOrEnchanted(card))
        {
            return;
        }

        Flash();
        await CreatureCmd.GainMaxHp(
            Owner.Creature,
            DynamicVars.MaxHp.BaseValue
        );
    }

    public override async Task AfterObtained()
    {
        int eligibleCards = CardUtils.GetUpgradedOrEnchantedCards(Owner).Count;

        if (eligibleCards == 0)
        {
            return;
        }

        Flash();
        await CreatureCmd.GainMaxHp(
            Owner.Creature,
            eligibleCards * DynamicVars.MaxHp.BaseValue
        );
    }
}