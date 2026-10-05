using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Artemis.Powers;
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
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class ShadowPounce() : HadesAncientsRelic(HadesAncient.Artemis)
{
    private int _attacksPlayedThisTurn;
    private bool _isActivating;
    private bool UsedThisTurn { get; set; }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress && (IsActivating || !UsedThisTurn);

    public override int DisplayAmount =>
        !IsActivating ? AttacksPlayedThisTurn % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new PowerVar<ShadowPouncePower>(1M)
    ];

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

    private int AttacksPlayedThisTurn
    {
        get => _attacksPlayedThisTurn;
        set
        {
            AssertMutable();
            _attacksPlayedThisTurn = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            int intValue = DynamicVars.Cards.IntValue;
            Status = AttacksPlayedThisTurn % intValue == intValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        AttacksPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        UsedThisTurn = false;
        UpdateDisplay();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Attack || UsedThisTurn)
            return;
        AttacksPlayedThisTurn++;
        int intValue = DynamicVars.Cards.IntValue;
        if (AttacksPlayedThisTurn % intValue != 0)
            return;
        _ = TaskHelper.RunSafely(DoActivateVisuals());

        await PowerCmd.Apply<ShadowPouncePower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(ShadowPouncePower)].BaseValue, Owner.Creature, null);
        UsedThisTurn = true;
        UpdateDisplay();
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        IsActivating = false;
        UsedThisTurn = false;
        UpdateDisplay();
        return Task.CompletedTask;
    }
}