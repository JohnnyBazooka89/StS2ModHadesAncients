using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Poseidon.SpireFields;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rewards;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace HadesAncients.HadesAncientsCode.Poseidon.Relics;

[Pool(typeof(EventRelicPool))]
public class SeaStar() : HadesAncientsRelic(HadesAncient.Poseidon)
{
    private const string RewardCopyPercentChangeKey = "RewardCopyPercentChange";

    private static readonly List<String> SoundPaths =
    [
        "sea_star/Poseidon [247].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [248].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [249].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [328].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [328].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [328].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [330].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [331].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [334].ogg".SoundPath(HadesAncient.Poseidon),
        "sea_star/Poseidon [338].ogg".SoundPath(HadesAncient.Poseidon),
    ];

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(RewardCopyPercentChangeKey, 30M),
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterRewardTaken(Player player, Reward reward)
    {
        if (player != Owner)
        {
            return;
        }

        int random = Owner.RunState.Rng.Niche.NextInt(0, 100);
        if (random >= DynamicVars[RewardCopyPercentChangeKey].BaseValue)
        {
            return;
        }

        Reward? newReward = GetSameTypeReward(reward);
        if (newReward == null)
        {
            return;
        }

        var rewardsSet = FindRewardsSetContaining(player, reward);
        if (rewardsSet == null)
            return;

        if (!newReward.IsPopulated)
        {
            newReward.Populate();
        }

        rewardsSet.Rewards.Add(newReward);

        if (LocalContext.IsMe(player))
        {
            PlaySound();
            AddRewardToCurrentScreen(newReward);
        }
    }

    private static Reward? GetSameTypeReward(Reward reward)
    {
        if (reward is CardReward cardReward)
        {
            var cardsWereManuallySet = cardReward._cardsWereManuallySet;
            var synchronizer = cardReward._synchronizer;
            if (cardsWereManuallySet)
            {
                var options = cardReward.Options;
                var rerollOptions = cardReward.RerollOptions;

                var originalCards = PoseidonSpireFields.SeaStarOriginalCards.Get(cardReward) ?? [];
                var cardsToOffer = originalCards
                    .Select<CardModel, CardModel>(card =>
                        cardReward.Player.RunState.CloneCard(card)
                    )
                    .ToList();

                return new CardReward(
                    cardsToOffer,
                    options.Source,
                    cardReward.Player,
                    rerollOptions,
                    synchronizer
                );
            }
            else
            {
                var options = cardReward.Options;
                var optionCount = cardReward.OptionCount;

                return new CardReward(
                    options,
                    optionCount,
                    cardReward.Player,
                    synchronizer
                );
            }
        }

        if (reward is CardRemovalReward cardRemovalReward)
        {
            return new CardRemovalReward(cardRemovalReward.Player);
        }

        if (reward is GoldReward goldReward)
        {
            return new GoldReward(
                goldReward.Amount,
                goldReward.Player,
                goldReward._wasGoldStolenBack
            );
        }

        if (reward is PotionReward potionReward)
        {
            return new PotionReward(potionReward.Player);
        }

        if (reward is RelicReward relicReward)
        {
            return new RelicReward(
                relicReward.Rarity,
                relicReward.Player
            );
        }

        if (reward is SpecialCardReward specialCardReward)
        {
            CardModel specialCard = specialCardReward._card;
            return new SpecialCardReward(
                specialCard,
                specialCardReward.Player
            );
        }

        return null;
    }

    private void PlaySound()
    {
        String soundPath = SoundPaths[Owner.RunState.Rng.Niche.NextInt(0, SoundPaths.Count)];

        if (HadesAncientsModConfig.PoseidonDisableSeaStarSoundEffects)
        {
            return;
        }

        float master = SaveManager.Instance.SettingsSave.VolumeMaster;
        float sfx = SaveManager.Instance.SettingsSave.VolumeSfx;

        float finalLinear = master * sfx;

        AudioStream sound = GD.Load<AudioStream>(soundPath);
        AudioStreamPlayer player = new()
        {
            Stream = sound,
            VolumeLinear = finalLinear
        };
        NGame.Instance.AddChild(player);
        player.Play();
        player.Finished += player.QueueFree;
    }

    private static void AddRewardToCurrentScreen(Reward newReward)
    {
        newReward.MarkContentAsSeen();

        var screen = NOverlayStack.Instance
            .GetChildren()
            .OfType<NRewardsScreen>()
            .LastOrDefault();

        if (screen == null)
            return;

        var rewardButtons = screen._rewardButtons;
        var rewardsContainer = screen._rewardsContainer;

        Control option;

        if (newReward is LinkedRewardSet linkedReward)
        {
            option = NLinkedRewardSet.Create(linkedReward, screen);
            option.Connect(
                NLinkedRewardSet.SignalName.RewardClaimed,
                Callable.From<NLinkedRewardSet>(screen.RewardCollectedFrom)
            );
        }
        else
        {
            var button = NRewardButton.Create(newReward, screen);

            button.Connect(
                NRewardButton.SignalName.RewardClaimed,
                Callable.From<NRewardButton>(screen.RewardCollectedFrom)
            );

            button.Connect(
                NRewardButton.SignalName.RewardSkipped,
                Callable.From<NRewardButton>(screen.RewardSkippedFrom)
            );

            option = button;
        }

        rewardButtons.Add(option);
        rewardsContainer.AddChildSafely(option);

        screen.UpdateScreenState();
        screen.TryEnableProceedButton();
    }

    private static RewardsSet? FindRewardsSetContaining(Player player, Reward reward)
    {
        var synchronizer = RunManager.Instance.RewardsSetSynchronizer;
        if (synchronizer == null)
            return null;

        var rewardStates = synchronizer._rewardStates;

        foreach (var playerState in rewardStates)
        {
            var rewardsStack = playerState.rewardsStack;

            foreach (var setState in rewardsStack)
            {
                var set = setState.set;

                if (set.Player == player && set.Rewards.Contains(reward))
                    return set;
            }
        }

        return null;
    }
}