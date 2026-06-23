using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Poseidon.Relics;

[Pool(typeof(EventRelicPool))]
public class WaveFlourish() : HadesAncientsRelic(HadesAncient.Poseidon)
{
    private int _cardsDrawn;
    private bool _isActivating;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override string FlashSfx => "event:/sfx/ui/relic_activate_draw";

    public override bool ShowCounter => (IsActivating || CardsDrawn < DynamicVars.Cards.IntValue) &&
                                        CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => !IsActivating ? CardsDrawn : DynamicVars.Cards.IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new EnergyVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
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
    public int CardsDrawn
    {
        get => _cardsDrawn;
        set
        {
            AssertMutable();
            _cardsDrawn = value;
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
            Status = CardsDrawn == intValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (fromHandDraw || card.Owner != Owner ||
            card.Owner.Creature.CombatState!.CurrentSide != card.Owner.Creature.Side)
        {
            return;
        }

        CardsDrawn++;
        int intValue = DynamicVars.Cards.IntValue;
        if (!CombatManager.Instance.IsInProgress || CardsDrawn != intValue)
        {
            return;
        }

        _ = TaskHelper.RunSafely(DoActivateVisuals());
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
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
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        CardsDrawn = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        CardsDrawn = 0;
        return Task.CompletedTask;
    }
}