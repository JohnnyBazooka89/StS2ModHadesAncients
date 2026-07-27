using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Hecate.SpireFields;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheFates() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
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
        new(CombatsKey, 5M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 21;
    }

    public override Task AfterModifyingRewards()
    {
        if (Owner.Creature.IsDead)
            return Task.CompletedTask;
        Charges--;
        return Task.CompletedTask;
    }

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner || Charges <= 0)
            return false;

        List<CardReward> cardRewards = rewards.OfType<CardReward>().ToList();

        if (cardRewards.Count == 0)
            return false;

        foreach (CardReward cardReward in cardRewards)
            HecateSpireFields.TheFuriesRerolls.Set(cardReward, HecateSpireFields.TheFuriesRerolls.Get(cardReward) + 1);
        Flash();
        return true;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars[CombatsKey].IntValue;
        return Task.CompletedTask;
    }
}