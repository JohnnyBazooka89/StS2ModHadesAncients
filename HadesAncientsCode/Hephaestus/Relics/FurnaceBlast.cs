using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hephaestus.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class FurnaceBlast() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlastPower>(8M),
        new PowerVar<VulnerablePower>(1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BlastPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return;

        List<Creature> targets = Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive)
            .ToList();

        if (targets.Count >= 1)
        {
            var lowestHp = targets.Min(c => c.CurrentHp);

            var lowestHpTargets = targets
                .Where(c => c.CurrentHp == lowestHp)
                .ToList();

            Owner.RunState.Rng.CombatTargets.Shuffle(lowestHpTargets);

            await PowerCmd.Apply<BlastPower>(new ThrowingPlayerChoiceContext(), lowestHpTargets[0],
                DynamicVars[nameof(BlastPower)].BaseValue, Owner.Creature, null);

            await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), lowestHpTargets[0],
                DynamicVars[nameof(VulnerablePower)].BaseValue, Owner.Creature, null);
            Flash();
        }
    }
}