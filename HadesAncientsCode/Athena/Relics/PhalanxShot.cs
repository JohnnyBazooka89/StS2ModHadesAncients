using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class PhalanxShot() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string BlockPercentKey = "BlockPercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => (int)GetDamageToDeal();

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(BlockPercentKey, 50M)
    ];

    private decimal GetDamageToDeal()
    {
        return Owner.Creature.Block * DynamicVars[BlockPercentKey].BaseValue / 100M;
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        decimal damageToDeal = GetDamageToDeal();
        if (!participants.Contains(Owner.Creature) || damageToDeal <= 0)
            return;

        Flash();

        foreach (Creature hittableEnemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            VfxCmd.PlayOnCreature(hittableEnemy, VfxCmd.slashPath);
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState!.HittableEnemies,
            damageToDeal, ValueProp.Unpowered, Owner.Creature);
    }

    public override Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature != Owner.Creature)
        {
            return Task.CompletedTask;
        }

        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterBlockCleared(Creature creature)
    {
        if (creature != Owner.Creature)
        {
            return Task.CompletedTask;
        }

        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}