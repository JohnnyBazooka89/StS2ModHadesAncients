using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class DivineIntervention() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string DamageTurnKey = "DamageTurn";
    private bool _isActivating;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => DisplayAmount > -1;

    public override int DisplayAmount
    {
        get
        {
            if (!CombatManager.Instance.IsInProgress || IsCanonical)
                return -1;
            int intValue = DynamicVars["DamageTurn"].IntValue;
            if (IsActivating)
                return intValue;
            int turnNumber = Owner.PlayerCombatState.TurnNumber;
            return turnNumber >= intValue ? -1 : turnNumber;
        }
    }

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

    public override IEnumerable<DynamicVar> CanonicalVars
        =>
        [
            new DamageVar(100M, ValueProp.Unpowered),
            new("DamageTurn", 8M)
        ];

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        if (Owner.PlayerCombatState.TurnNumber == DynamicVars["DamageTurn"].IntValue)
            Status = RelicStatus.Active;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature))
            return;
        int intValue = DynamicVars["DamageTurn"].IntValue;
        int turnNumber = Owner.PlayerCombatState.TurnNumber;
        Status = RelicStatus.Normal;
        if (turnNumber != intValue)
            return;
        TaskHelper.RunSafely(DoActivateVisuals());

        if (Owner.RunState.CurrentRoom.RoomType == RoomType.Boss)
        {
            await CreatureCmd.Damage(choiceContext,
                Owner.Creature.CombatState.HittableEnemies,
                DynamicVars.Damage, Owner.Creature);
            InvokeDisplayAmountChanged();
        }
        else
        {
            await KillEnemies(Owner.Creature.CombatState.Enemies.ToList());
        }
    }

    private async Task KillEnemies(List<Creature> creatures)
    {
        foreach (Creature creature in creatures)
        {
            creature.RemoveAllPowersInternalExcept();
            await CreatureCmd.Kill(creature);
        }

        await CombatManager.Instance.CheckWinCondition();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        Status = RelicStatus.Normal;
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
}