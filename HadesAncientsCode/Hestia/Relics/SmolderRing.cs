using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class SmolderRing() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private int _damageToDeal;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => DamageToDeal;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2, ValueProp.Unpowered)];

    private int DamageToDeal
    {
        get => _damageToDeal;
        set
        {
            AssertMutable();
            _damageToDeal = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack)
        {
            return Task.CompletedTask;
        }

        DamageToDeal += DynamicVars.Damage.IntValue;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature) || DamageToDeal <= 0)
            return;

        foreach (Creature hittableEnemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            NFireBurstVfx? child = NFireBurstVfx.Create(hittableEnemy, 0.75f);
            NCombatRoom? instance = NCombatRoom.Instance;
            instance?.CombatVfxContainer.AddChildSafely(child);
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState!.HittableEnemies,
            DamageToDeal, ValueProp.Unpowered, Owner.Creature);
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        DamageToDeal = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        DamageToDeal = 0;
        return Task.CompletedTask;
    }
}