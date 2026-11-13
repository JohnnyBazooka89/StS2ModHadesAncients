using BaseLib.Hooks;
using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class WinterHarvest() : HadesAncientsRelic(HadesAncient.Demeter), IHealthBarForecastSource
{
    private const string HealthThresholdKey = "HealthThreshold";
    private static readonly Color Color = Colors.Snow;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HealthThresholdKey, 25M)
    ];

    public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        if (Owner.GetRelic<WinterHarvest>() == null ||
            context.CombatState?.HittableEnemies.Contains(context.Creature) != true)
            return [];

        decimal damage = context.Creature.MaxHp * DynamicVars[HealthThresholdKey].BaseValue / 100M;
        return
        [
            new HealthBarForecastSegment(
                PowerUtils.GetDamageForForecast(Owner.Creature, context.Creature, damage, ValueProp.Unpowered),
                Color,
                HealthBarForecastDirection.FromLeft)
        ];
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner.Creature.IsDead)
            return Task.CompletedTask;

        HealthBarForecastRegistry.Register(HadesAncientsMainFile.ModId, Id.Entry + Owner.NetId, this);
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        List<Creature> targets = combatState.HittableEnemies
            .Where(participants.Contains)
            .ToList();

        foreach (Creature target in targets)
        {
            if (!target.IsAlive ||
                target.CurrentHp > target.MaxHp * DynamicVars[HealthThresholdKey].BaseValue / 100M)
                continue;

            Flash();
            await CreatureCmd.Kill(target);
        }
    }

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        ICombatState? combatState = Owner.Creature.CombatState;
        if (wasRemovalPrevented || combatState == null || creature.CombatState != combatState)
            return;

        Flash();
        await CreatureCmd.Damage(
            choiceContext,
            combatState.HittableEnemies,
            creature.MaxHp,
            ValueProp.Unpowered,
            Owner.Creature);
    }
}
