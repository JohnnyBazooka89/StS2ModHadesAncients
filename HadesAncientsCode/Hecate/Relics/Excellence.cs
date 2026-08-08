using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Excellence() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string CombatsKey = "Combats";
    private int _combatsSeen;
    private bool _isActivating;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override int DisplayAmount =>
        !IsActivating ? CombatsSeen % DynamicVars[CombatsKey].IntValue : DynamicVars[CombatsKey].IntValue;


    public override bool ShowCounter => true;

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

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

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(CombatsKey, 3M)
    ];

    private bool IsInTriggeringCombat => CombatsSeen > 0 &&
                                         CombatsSeen % DynamicVars[CombatsKey].BaseValue ==
                                         DynamicVars[CombatsKey].BaseValue - 1;

    public int GetArcanaRelicNumber()
    {
        return 19;
    }

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> rewardOptions,
        CardCreationOptions creationOptions)
    {
        if (Owner != player || creationOptions.Source != CardCreationSource.Encounter || !IsInTriggeringCombat ||
            !creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward) ||
            !creationOptions.Flags.HasFlag(CardCreationFlags.IsFromCombat))
            return false;
        bool allowDupes = false;
        List<CardModel> list = creationOptions.GetPossibleCards(player).ToList();
        IEnumerable<CardModel> source =
            list.Where((Func<CardModel, bool>)(c => CardPoolFilter(c, rewardOptions, false))).ToList();
        if (!source.Any())
        {
            allowDupes = true;
            source = list.Where(c => CardPoolFilter(c, rewardOptions, true));
        }

        if (!source.Any())
            return false;

        CardModel? card = CardFactory.CreateForReward(Owner, 1, new CardCreationOptions(creationOptions.CardPools,
                CardCreationSource.Other, creationOptions.RarityOdds, (Func<CardModel, bool>)(c =>
                {
                    Func<CardModel, bool>? cardPoolFilter = creationOptions.CardPoolFilter;
                    return (cardPoolFilter == null || cardPoolFilter(c)) &&
                           CardPoolFilter(c, rewardOptions, allowDupes);
                })).WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications))
            .FirstOrDefault()
            ?.Card;
        if (card != null)
        {
            CardCreationResult cardCreationResult = new CardCreationResult(card);
            cardCreationResult.ModifyCard(card, this);
            rewardOptions.Add(cardCreationResult);
        }

        return card != null;
    }

    public override Task BeforeCombatRewardOffered(RewardsSet rewards, CombatRoom room)
    {
        if (rewards.Player != Owner || rewards.Rewards.All(r => r is not CardReward) ||
            room.Encounter.RoomType != RoomType.Monster)
            return Task.CompletedTask;
        if (IsInTriggeringCombat)
            TaskHelper.RunSafely(DoActivateVisuals());
        ++CombatsSeen;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    private static bool CardPoolFilter(
        CardModel card,
        List<CardCreationResult> rewardOptions,
        bool allowDupes)
    {
        if (card.Rarity != CardRarity.Rare)
            return false;
        return allowDupes ||
               rewardOptions.TrueForAll((Predicate<CardCreationResult>)(o => o.originalCard.Id != card.Id));
    }
}