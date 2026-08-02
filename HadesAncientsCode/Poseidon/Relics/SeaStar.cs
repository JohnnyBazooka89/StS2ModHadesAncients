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

    private static readonly List<string> SoundPaths =
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
        new(RewardCopyPercentChangeKey, 40M),
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override Task AfterRewardTaken(Player player, Reward reward)
    {
        if (player != Owner)
        {
            return Task.CompletedTask;
        }

        int random = Owner.RunState.Rng.Niche.NextInt(0, 100);
        if (random >= DynamicVars[RewardCopyPercentChangeKey].BaseValue)
        {
            return Task.CompletedTask;
        }

        Reward? newReward = GetSameTypeReward(reward);
        if (newReward == null)
        {
            return Task.CompletedTask;
        }

        var rewardsSet = FindRewardsSetContaining(player, reward);
        if (rewardsSet == null)
            return Task.CompletedTask;

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

        return Task.CompletedTask;
    }

    private static Reward? GetSameTypeReward(Reward reward)
    {
        if (reward is CardReward cardReward)
        {
            bool cardsWereManuallySet = cardReward._cardsWereManuallySet;
            var synchronizer = cardReward._synchronizer;

            if (cardsWereManuallySet)
            {
                var options = cardReward.Options;
                var rerollOptions = cardReward.RerollOptions;

                var originalCards = PoseidonSpireFields.SeaStarOriginalCards.Get(cardReward) ?? [];

                var cardsToOffer = originalCards
                    .Select(card => cardReward.Player.RunState.CloneCard(card))
                    .ToList();

                return new CardReward(
                    cardsToOffer,
                    options.Source,
                    cardReward.Player,
                    rerollOptions,
                    synchronizer
                );
            }

            return new CardReward(
                cardReward.Options,
                cardReward.OptionCount,
                cardReward.Player,
                synchronizer
            );
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
            CardModel? specialCard = specialCardReward._card;

            if (specialCard == null)
            {
                return null;
            }

            CardModel clonedCard = specialCardReward.Player.RunState.CloneCard(specialCard);

            return new SpecialCardReward(
                clonedCard,
                specialCardReward.Player
            );
        }

        return null;
    }

    private void PlaySound()
    {
        if (HadesAncientsModConfig.PoseidonDisableSeaStarSoundEffects)
        {
            return;
        }

        string soundPath = SoundPaths[Random.Shared.Next(SoundPaths.Count)];

        float master = SaveManager.Instance.SettingsSave.VolumeMaster;
        float sfx = SaveManager.Instance.SettingsSave.VolumeSfx;

        AudioStream sound = GD.Load<AudioStream>(soundPath);

        AudioStreamPlayer audioPlayer = new()
        {
            Stream = sound,
            VolumeLinear = master * sfx
        };

        NGame.Instance?.AddChild(audioPlayer);
        audioPlayer.Play();
        audioPlayer.Finished += audioPlayer.QueueFree;
    }

    private static void AddRewardToCurrentScreen(Reward newReward)
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
    
    private static RewardsSet? FindRewardsSetContaining(Player player, Reward reward)
    {
        var synchronizer = RunManager.Instance.RewardsSetSynchronizer;
        if (synchronizer == null) return null;
        var rewardStates = synchronizer._rewardStates;
        foreach (var playerState in rewardStates)
        {
            var rewardsStack = playerState.rewardsStack;
            foreach (var setState in rewardsStack)
            {
                var set = setState.set;
                if (set.Player == player && set.Rewards.Contains(reward)) return set;
            }
        }

        return null;
    }
}