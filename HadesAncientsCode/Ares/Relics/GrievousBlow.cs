using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class GrievousBlow() : HadesAncientsRelic(HadesAncient.Ares)
{
    private const int AttacksThreshold = 3;
    private int _attacksPlayed;
    private CardModel? _attackToDouble;
    private bool _isActivating;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? AttacksPlayed % AttacksThreshold : AttacksThreshold;

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
    private int AttacksPlayed
    {
        get => _attacksPlayed;
        set
        {
            AssertMutable();
            _attacksPlayed = value % AttacksThreshold;
            UpdateDisplay();
        }
    }

    private CardModel? AttackToDouble
    {
        get => _attackToDouble;
        set
        {
            AssertMutable();
            _attackToDouble = value;
        }
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
            Status = RelicStatus.Normal;
        else
            Status = AttacksPlayed == AttacksThreshold - 1 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    private void NotifyAttackPlayed()
    {
        ++AttacksPlayed;
        if (AttacksPlayed != 0)
            return;
        TaskHelper.RunSafely(DoActivateVisuals());
    }

    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || dealer != Owner.Creature && dealer != Owner.Osty)
            return 1M;
        if (AttackToDouble == null)
        {
            return cardSource.Pile is not { Type: PileType.Play } &&
                   AttacksPlayed == AttacksThreshold - 1
                ? 2M
                : 1M;
        }

        return cardSource == AttackToDouble ? 2M : 1M;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;
        NotifyAttackPlayed();
        if (AttacksPlayed == 0)
            AttackToDouble = cardPlay.Card;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (AttackToDouble == null || cardPlay.Card != AttackToDouble)
            return Task.CompletedTask;
        AttackToDouble = null;
        return Task.CompletedTask;
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}