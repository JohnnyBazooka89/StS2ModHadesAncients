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
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class HeavyMetal() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ForgeArmorPower>(4M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ForgeArmorPower>()
    ];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return;

        List<Creature> targets = Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive).ToList();
        await PowerCmd.Apply<ForgeArmorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars[nameof(ForgeArmorPower)].BaseValue, Owner.Creature, null);
        Flash();
        await Cmd.Wait(0.25f);

        if (targets.Count >= 1)
        {
            Owner.RunState.Rng.CombatTargets.Shuffle(targets);
            int damage = Owner.Creature.GetPowerAmount<ForgeArmorPower>();
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), targets[0], damage, ValueProp.Unpowered,
                Owner.Creature);
        }
    }
}