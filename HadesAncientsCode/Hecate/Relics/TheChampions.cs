using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.LinkedRewards.LinkedRewardSet;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheChampions() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string RelicRewardsKey = "RelicRewards";
    private const string RelicsToOfferKey = "RelicsToOffer";
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
        new(RelicRewardsKey, 3M),
        new(RelicsToOfferKey, 2M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 22;
    }

    public override Task AfterModifyingRewards()
    {
        if (Owner.Creature.IsDead)
            return Task.CompletedTask;
        Charges--;
        return Task.CompletedTask;
    }

    public override bool TryModifyRewardsLate(
        Player player,
        List<Reward> rewards,
        AbstractRoom? room)
    {
        if (player != Owner || Charges <= 0 || room is not { RoomType: RoomType.Elite })
            return false;

        var modified = false;

        for (var i = 0; i < rewards.Count; i++)
        {
            if (rewards[i] is not RelicReward relicReward)
                continue;

            List<Reward> linkedRelicRewards = [relicReward];

            for (int j = 1; j < DynamicVars[RelicsToOfferKey].IntValue; j++)
            {
                linkedRelicRewards.Add(new RelicReward(Owner));
            }

            rewards[i] = new CustomLinkedRewardSet(linkedRelicRewards, Owner);

            modified = true;
        }

        if (modified)
            Flash();

        return modified;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars[RelicRewardsKey].IntValue;
        return Task.CompletedTask;
    }
}