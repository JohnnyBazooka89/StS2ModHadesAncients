using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheEnchantress() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string CombatsKey = "Combats";
    private int _charges;

    [SavedProperty]
    private int Charges
    {
        get => _charges;
        set
        {
            AssertMutable();
            _charges = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override int DisplayAmount => Charges;

    public override bool ShowCounter => true;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(CombatsKey, 2M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 16;
    }

    /**
     * This list intentionally excludes:
     * - Clone - the Clone option doesn't appear at Rest Sites without Pael's Growth.
     * - Goopy - it only works with basic Defends.
     * - Inky - it doesn't check whether a card is an Attack and doesn't work correctly with some Attacks, e.g. Sword Boomerang.
     * - Slumbering Essence - it's unused in the base game.
     * - Spiral - it only works with basic Attacks and Defends.
     * - DeprecatedEnchantment - it's used as a fallback when loading saves that reference missing enchantments.
     * - MockFreeEnchantment - it appears to be unused in the base game and is likely intended for testing only.
     */
    private static List<EnchantmentOption> CreateEnchantmentPool()
    {
        return
        [
            new EnchantmentOption(ModelDb.Enchantment<Adroit>(), 3M),
            new EnchantmentOption(ModelDb.Enchantment<Corrupted>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Glam>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Imbued>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Instinct>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Momentum>(), 5M),
            new EnchantmentOption(ModelDb.Enchantment<Nimble>(), 2M),
            new EnchantmentOption(ModelDb.Enchantment<PerfectFit>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<RoyallyApproved>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Sharp>(), 2M),
            new EnchantmentOption(ModelDb.Enchantment<Slither>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<SoulsPower>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Sown>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Steady>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Swift>(), 2M),
            new EnchantmentOption(ModelDb.Enchantment<TezcatarasEmber>(), 1M),
            new EnchantmentOption(ModelDb.Enchantment<Vigorous>(), 8M)
        ];
    }

    public override Task AfterModifyingCardRewardOptions()
    {
        if (Owner.Creature.IsDead)
            return Task.CompletedTask;
        Charges--;
        return Task.CompletedTask;
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != Owner
            || !options.Flags.HasFlag(CardCreationFlags.IsCardReward)
            || Charges <= 0)
        {
            return false;
        }

        List<EnchantmentOption> enchantmentPool = CreateEnchantmentPool();

        var rng = Owner.RunState.Rng.CombatCardGeneration;

        foreach (CardCreationResult cardReward in cardRewards)
        {
            CardModel card = cardReward.Card;

            List<EnchantmentOption> compatibleEnchantments = enchantmentPool
                .Where(option => option.Enchantment.CanEnchant(card))
                .ToList();

            if (compatibleEnchantments.Count == 0)
                continue;

            rng.Shuffle(compatibleEnchantments);

            EnchantmentOption selected = compatibleEnchantments[0];

            CardModel enchantedCard = Owner.RunState.CloneCard(card);

            CardCmd.Enchant(
                selected.Enchantment.ToMutable(),
                enchantedCard,
                selected.Amount);

            cardReward.ModifyCard(enchantedCard, this);
        }

        return true;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars[CombatsKey].IntValue;
        return Task.CompletedTask;
    }

    private sealed record EnchantmentOption(
        EnchantmentModel Enchantment,
        decimal Amount);
}