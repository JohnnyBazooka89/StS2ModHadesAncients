using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Night() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private int _attacksPlayed;
    private bool _isActivating;
    private CardModel? _pendingCardToActivate;
    private bool _usedThisCombat;

    private bool UsedThisCombat
    {
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
            UpdateDisplay();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override string FlashSfx => "event:/sfx/ui/relic_activate_draw";

    public override bool ShowCounter =>
        (IsActivating || (AttacksPlayed < DynamicVars.Cards.IntValue && !UsedThisCombat)) &&
        CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => !IsActivating ? AttacksPlayed : DynamicVars.Cards.IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    private int AttacksPlayed
    {
        get => _attacksPlayed;
        set
        {
            AssertMutable();
            _attacksPlayed = value;
            UpdateDisplay();
        }
    }

    public int GetArcanaRelicNumber()
    {
        return 10;
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            Status = !UsedThisCombat && AttacksPlayed == DynamicVars.Cards.IntValue - 1
                ? RelicStatus.Active
                : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack || cardPlay.IsAutoPlay ||
            !cardPlay.IsFirstInSeries || UsedThisCombat)
        {
            return Task.CompletedTask;
        }

        AttacksPlayed++;
        return Task.CompletedTask;
    }

    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation cardLocation)
    {
        _pendingCardToActivate = null;

        if (!isAutoPlay
            && !UsedThisCombat
            && CombatManager.Instance.IsInProgress
            && AttacksPlayed == DynamicVars.Cards.IntValue - 1
            && card.Owner == Owner
            && card.Type == CardType.Attack)
        {
            _pendingCardToActivate = card;
        }

        return cardLocation;
    }

    public override int ModifyCardPlayCount(
        CardModel card,
        Creature? target,
        int playCount)
    {
        if (!ReferenceEquals(card, _pendingCardToActivate))
            return playCount;

        return playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        UsedThisCombat = true;
        return Task.CompletedTask;
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || combatState.RoundNumber != 1)
        {
            return Task.CompletedTask;
        }

        AttacksPlayed = 0;
        UsedThisCombat = false;
        _pendingCardToActivate = null;

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        AttacksPlayed = 0;
        UsedThisCombat = false;
        _pendingCardToActivate = null;

        return Task.CompletedTask;
    }
}