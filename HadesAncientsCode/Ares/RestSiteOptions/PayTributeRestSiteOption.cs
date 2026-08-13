using BaseLib.Common.Rewards;
using HadesAncients.HadesAncientsCode.Ares.SpireFields;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;

namespace HadesAncients.HadesAncientsCode.Ares.RestSiteOptions;

public class PayTributeRestSiteOption(Player owner, PayTributeParams payTributeParams)
    : HadesAncientsRestSiteOption(HadesAncient.Ares, owner)
{
    public override string OptionId => "PAY_TRIBUTE";

    public override LocString Description
    {
        get
        {
            LocString description = new LocString("static_hover_tips", "HADESANCIENTS-PAY_TRIBUTE.description");
            description.Add(PayTributeParams.MaxHpLossKey, payTributeParams.MaxHpLoss);
            description.Add(PayTributeParams.RelicsToObtainKey, payTributeParams.RelicsToObtain);
            description.Add(PayTributeParams.CardsToUpgradeKey, payTributeParams.CardsToUpgrade);
            description.Add(PayTributeParams.CardsToRemoveKey, payTributeParams.CardsToRemove);
            description.Add(PayTributeParams.PotionsToProcureKey, payTributeParams.PotionsToProcure);
            return description;
        }
    }

    public override async Task<bool> OnSelect()
    {
        await CreatureCmdUtils.LoseMaxHpSafely(new ThrowingPlayerChoiceContext(), Owner.Creature,
            payTributeParams.MaxHpLoss, false);

        await RewardsCmd.OfferCustom(Owner, GenerateRewards());

        return true;
    }

    private List<Reward> GenerateRewards()
    {
        List<Reward> rewards = [];

        for (int i = 0; i < payTributeParams.RelicsToObtain; i++)
        {
            RelicReward reward = new RelicReward(Owner);
            AresSpireFields.PayTributeRewardsSetIndex.Set(reward, 0);
            rewards.Add(reward);
        }

        CardUpgradeReward upgradeReward = new CardUpgradeReward(Owner)
        {
            Amount = payTributeParams.CardsToUpgrade
        };

        AresSpireFields.PayTributeRewardsSetIndex.Set(upgradeReward, 1);
        rewards.Add(upgradeReward);

        for (int i = 0; i < payTributeParams.CardsToRemove; i++)
        {
            CardRemovalReward reward = new CardRemovalReward(Owner);
            AresSpireFields.PayTributeRewardsSetIndex.Set(reward, 2);
            rewards.Add(reward);
        }

        for (int i = 0; i < payTributeParams.PotionsToProcure; i++)
        {
            PotionReward reward = new PotionReward(Owner);
            AresSpireFields.PayTributeRewardsSetIndex.Set(reward, 3);
            rewards.Add(reward);
        }

        return rewards;
    }
}

public sealed record PayTributeParams(
    int MaxHpLoss,
    int RelicsToObtain,
    int CardsToUpgrade,
    int CardsToRemove,
    int PotionsToProcure
)
{
    public const string MaxHpLossKey = "MaxHpLoss";
    public const string RelicsToObtainKey = "RelicsToObtain";
    public const string CardsToUpgradeKey = "CardsToUpgrade";
    public const string CardsToRemoveKey = "CardsToRemove";
    public const string PotionsToProcureKey = "PotionsToProcure";
}