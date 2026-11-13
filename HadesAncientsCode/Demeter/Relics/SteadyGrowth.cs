using BaseLib.Common.Rewards;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Rewards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class SteadyGrowth() : HadesAncientsRelic(HadesAncient.Demeter)
{
    private const string CombatsKey = "Combats";
    private const string CardsToUpgradeKey = "CardsToUpgrade";
    private const string CardsToEnchantKey = "CardsToEnchant";
    private int _combatsSeen;

    [SavedProperty]
    private int CombatsSeen
    {
        get => _combatsSeen;
        set
        {
            AssertMutable();
            _combatsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override int DisplayAmount => CombatsSeen % DynamicVars[CombatsKey].IntValue;

    public override bool ShowCounter => true;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(CombatsKey, 2M),
        new(CardsToUpgradeKey, 1M),
        new(CardsToEnchantKey, 1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromEnchantment<Steady>()
    ];

    public override Task AfterCombatEnd(CombatRoom room)
    {
        CombatsSeen++;
        room.AddExtraReward(Owner, new CardUpgradeReward(Owner)
        {
            Amount = DynamicVars[CardsToUpgradeKey].IntValue
        });

        if (CombatsSeen % DynamicVars[CombatsKey].IntValue == 0)
        {
            Flash();
            room.AddExtraReward(Owner, new CardEnchantReward(
                Owner,
                ModelDb.Enchantment<Steady>(),
                DynamicVars[CardsToEnchantKey].IntValue));
        }

        return Task.CompletedTask;
    }
}