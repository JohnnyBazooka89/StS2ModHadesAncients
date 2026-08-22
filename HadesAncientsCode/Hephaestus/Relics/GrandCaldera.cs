using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class GrandCaldera() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    private const string TurnsKey = "Turns";
    private bool _isActivating;
    private int _turnsSeen;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? TurnsSeen : DynamicVars[TurnsKey].IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnsKey, 3M),
        new DamageVar(24, ValueProp.Unpowered),
        new PowerVar<VulnerablePower>(1M),
        new PowerVar<WeakPower>(1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
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

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner)
            return;
        TurnsSeen = (TurnsSeen + 1) % DynamicVars[TurnsKey].IntValue;
        Status = TurnsSeen == DynamicVars[TurnsKey].IntValue - 1
            ? RelicStatus.Active
            : RelicStatus.Normal;
        if (TurnsSeen != 0)
            return;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        VfxCmd.PlayOnCreatures(Owner.Creature.CombatState!.HittableEnemies, VfxCmd.bluntPath);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState!.HittableEnemies,
            DynamicVars.Damage.IntValue, ValueProp.Unpowered, Owner.Creature);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, Owner.Creature.CombatState!.HittableEnemies,
            DynamicVars.Vulnerable.BaseValue, Owner.Creature, null);
        await PowerCmd.Apply<WeakPower>(choiceContext, Owner.Creature.CombatState!.HittableEnemies,
            DynamicVars.Weak.BaseValue, Owner.Creature, null);
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