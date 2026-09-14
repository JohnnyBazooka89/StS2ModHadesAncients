using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HadesAncients.HadesAncientsCode.Zeus.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Zeus.Relics;

[Pool(typeof(EventRelicPool))]
public class StormRing() : HadesAncientsRelic(HadesAncient.Zeus), IShouldPlayTargeting
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private Creature? CreatureToTarget { get; set; }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public bool ShouldPlayTargeting(CardModel card, Creature? cardTarget)
    {
        if (card.Owner != Owner)
        {
            return true;
        }

        IReadOnlyList<Creature> enemies = GetValidEnemies();

        if (cardTarget == null || !enemies.Contains(cardTarget))
        {
            return true;
        }

        Creature? lockedTarget = CreatureToTarget;

        if (lockedTarget == null || !enemies.Contains(lockedTarget) || !card.IsValidTarget(lockedTarget))
        {
            return true;
        }

        return cardTarget == lockedTarget;
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature? target = cardPlay.Target;

        if (target == null || cardPlay.IsAutoPlay || cardPlay.Card.Owner != Owner)
        {
            return;
        }

        IReadOnlyList<Creature>? enemies = GetValidEnemies();

        if (enemies == null || !enemies.Contains(target))
        {
            return;
        }

        foreach (Creature enemy in enemies)
        {
            if (enemy != target)
            {
                MarkedByStormRingPower? powerOnAnotherEnemy = enemy.GetPower<MarkedByStormRingPower>();
                if (powerOnAnotherEnemy != null && powerOnAnotherEnemy.Applier == Owner.Creature)
                {
                    await PowerCmd.Remove<MarkedByStormRingPower>(enemy);
                }
            }
        }

        CreatureToTarget = target;

        if (!target.HasPower<MarkedByStormRingPower>())
        {
            await PowerCmd.Apply<MarkedByStormRingPower>(
                choiceContext,
                target,
                1M,
                Owner.Creature,
                cardPlay.Card
            );
        }
    }

    private IReadOnlyList<Creature> GetValidEnemies()
    {
        return Owner.Creature.CombatState!.HittableEnemies
            .Where(enemy => enemy.GetCreatureNode()?.IsInteractable ?? false)
            .ToList();
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return Task.CompletedTask;

        CreatureToTarget = null;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        CreatureToTarget = null;
        return Task.CompletedTask;
    }
}