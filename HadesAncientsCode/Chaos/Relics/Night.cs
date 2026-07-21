using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Chaos.Relics.Types;
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
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Chaos.Relics;

[Pool(typeof(EventRelicPool))]
public class Night() : HadesAncientsRelic(HadesAncient.Chaos), IArcanaRelic
{
    private int _attacksPlayed;
    private bool _isActivating;
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

    [SavedProperty]
    public int AttacksPlayed
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
            int requiredAttacks = DynamicVars.Cards.IntValue;

            Status = !UsedThisCombat && AttacksPlayed == requiredAttacks - 1
                ? RelicStatus.Active
                : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack)
        {
            return Task.CompletedTask;
        }

        AttacksPlayed++;
        int intValue = DynamicVars.Cards.IntValue;
        if (!CombatManager.Instance.IsInProgress || AttacksPlayed != intValue)
            return Task.CompletedTask;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        UsedThisCombat = true;
        return Task.CompletedTask;
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        int intValue = DynamicVars.Cards.IntValue;

        return UsedThisCombat
               || !CombatManager.Instance.IsInProgress
               || AttacksPlayed != intValue - 1
               || card.Type != CardType.Attack
               || card.Owner.Creature != Owner.Creature
            ? playCount
            : playCount + 1;
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

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        AttacksPlayed = 0;
        UsedThisCombat = false;
        return Task.CompletedTask;
    }
}