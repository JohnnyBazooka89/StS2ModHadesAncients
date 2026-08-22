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

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class ProfuseBleeding() : HadesAncientsRelic(HadesAncient.Ares), IHealthBarForecastSource
{
    private static readonly Color Color = new(120 / 255f, 6 / 255f, 6 / 255f);

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8M, ValueProp.Unblockable | ValueProp.Unpowered)
    ];

    public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        List<Creature> targets = context.CombatState?.Enemies.Where(c => c.IsAlive).ToList() ?? [];

        if (!targets.Contains(context.Creature))
        {
            return [];
        }

        int hpLoss = PowerUtils.GetUniqueDebuffsCount(context.Creature) * DynamicVars.Damage.IntValue;

        return
        [
            new HealthBarForecastSegment(hpLoss, Color, HealthBarForecastDirection.FromRight)
        ];
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner.Creature.IsDead)
        {
            return Task.CompletedTask;
        }

        HealthBarForecastRegistry.Register(HadesAncientsMainFile.ModId, Id.Entry, this);

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        List<Creature> targets = combatState.GetOpponentsOf(Owner.Creature).Where(c => c.IsAlive).ToList();

        foreach (Creature target in targets)
        {
            if (!participants.Contains(target))
            {
                continue;
            }

            int hpLoss = PowerUtils.GetUniqueDebuffsCount(target) * DynamicVars.Damage.IntValue;

            if (hpLoss <= 0)
            {
                continue;
            }

            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, hpLoss, DynamicVars.Damage.Props,
                target);
        }
    }
}