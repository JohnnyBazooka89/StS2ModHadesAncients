using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rewards;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rewards;

namespace HadesAncients.HadesAncientsCode.Shared.Utils;

public class RewardUtils
{
    public static void AddRewardToCurrentScreen(Reward newReward)
    {
        newReward.MarkContentAsSeen();

        NRewardsScreen? screen = NOverlayStack.Instance?
            .GetChildren()
            .OfType<NRewardsScreen>()
            .LastOrDefault();

        if (screen == null)
        {
            return;
        }

        var button = NRewardButton.Create(newReward, screen);

        button.Connect(
            NRewardButton.SignalName.RewardClaimed,
            Callable.From<NRewardButton>(screen.RewardCollectedFrom)
        );

        button.Connect(
            NRewardButton.SignalName.RewardSkipped,
            Callable.From<NRewardButton>(screen.RewardSkippedFrom)
        );

        screen._rewardButtons.Add(button);
        screen._rewardsContainer.AddChildSafely(button);

        screen.UpdateScreenState();
        screen.TryEnableProceedButton();
    }
}