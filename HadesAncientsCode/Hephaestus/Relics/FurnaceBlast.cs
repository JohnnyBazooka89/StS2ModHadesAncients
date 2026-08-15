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
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class FurnaceBlast() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    private const string BlastIncreaseKey = "BlastIncrease";
    private int _blastToApply;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => BlastToApply;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlastPower>(6M),
        new(BlastIncreaseKey, 1M)
    ];

    private int BlastToApply
    {
        get => _blastToApply;
        set
        {
            AssertMutable();
            _blastToApply = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BlastPower>()
    ];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
        {
            return;
        }

        if (Owner.PlayerCombatState!.TurnNumber == 1)
        {
            BlastToApply = DynamicVars[nameof(BlastPower)].IntValue;
        }

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
                BlastToApply, Owner.Creature, null);
            Flash();
        }

        BlastToApply += DynamicVars[BlastIncreaseKey].IntValue;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}