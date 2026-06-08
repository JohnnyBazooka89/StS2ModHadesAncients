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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class DivineProtection() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string TurnsKey = "Turns";
    private bool _isActivating;
    private int _turnsSeen;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BufferPower>()
    ];
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? TurnsSeen : DynamicVars[TurnsKey].IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnsKey, 3M),
        new PowerVar<BufferPower>(1),
    ];

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
    private int TurnsSeen
    {
        get => _turnsSeen;
        set
        {
            AssertMutable();
            _turnsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return;
        TurnsSeen = (TurnsSeen + 1) % DynamicVars[TurnsKey].IntValue;
        Status = TurnsSeen == DynamicVars[TurnsKey].IntValue - 1
            ? RelicStatus.Active
            : RelicStatus.Normal;
        if (TurnsSeen != 0)
            return;
        TaskHelper.RunSafely(DoActivateVisuals());
        await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars[nameof(BufferPower)].BaseValue,
            Owner.Creature, null);
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
        return Task.CompletedTask;
    }
}