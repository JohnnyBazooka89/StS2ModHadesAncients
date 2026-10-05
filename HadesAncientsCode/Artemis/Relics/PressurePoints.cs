using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class PressurePoints() : HadesAncientsRelic(HadesAncient.Artemis), IModifyDamageMultiplicativeCompatibility
{
    private const string MoreDamagePercentKey = "MoreDamagePercent";

    private const int AttacksThreshold = 5;
    private int _attacksPlayed;
    private CardModel? _attackToIncreaseDamage;
    private bool _isActivating;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? AttacksPlayed % AttacksThreshold : AttacksThreshold;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MoreDamagePercentKey, 150M)
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

    private CardModel? AttackToIncreaseDamage
    {
        get => _attackToIncreaseDamage;
        set
        {
            AssertMutable();
            _attackToIncreaseDamage = value;
        }
    }

    public decimal ModifyDamageMultiplicativeCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || dealer != Owner.Creature && dealer != Owner.Osty)
            return 1M;

        decimal increasedDamageMultiplier = 1 + DynamicVars[MoreDamagePercentKey].BaseValue / 100M;

        if (AttackToIncreaseDamage == null)
        {
            return cardSource.Pile is not { Type: PileType.Play } &&
                   AttacksPlayed == AttacksThreshold - 1
                ? increasedDamageMultiplier
                : 1M;
        }

        return cardSource == AttackToIncreaseDamage ? increasedDamageMultiplier : 1M;
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

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;
        NotifyAttackPlayed();
        if (AttacksPlayed == 0)
            AttackToIncreaseDamage = cardPlay.Card;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (AttackToIncreaseDamage == null || cardPlay.Card != AttackToIncreaseDamage)
            return Task.CompletedTask;
        AttackToIncreaseDamage = null;
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