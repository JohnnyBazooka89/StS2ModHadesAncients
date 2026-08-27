using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hestia.Cards;

[Pool(typeof(EventCardPool))]
public class Fireball()
    : HadesAncientsCard(HadesAncient.Hestia, 2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    private const string DamageIncreaseKey = "DamageIncrease";

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14M, ValueProp.Move),
        new(DamageIncreaseKey, 7M)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tasks = new List<Task>();

        foreach (Creature hittableEnemy in CombatState!.HittableEnemies)
        {
            NCombatRoom? instance = NCombatRoom.Instance;
            NCreature? creatureNode = instance?.GetCreatureNode(hittableEnemy);

            if (creatureNode == null)
                continue;

            NLargeMagicMissileVfx child =
                NLargeMagicMissileVfx.Create(
                    creatureNode.GetBottomOfHitbox(),
                    new Color("50b598"))!;

            instance?.CombatVfxContainer.AddChildSafely(child);

            tasks.Add(Cmd.Wait(child.WaitTime));
        }

        await Task.WhenAll(tasks);
        await Cmd.Wait(0.25f);

        foreach (Creature hittableEnemy in CombatState!.HittableEnemies)
        {
            NCombatRoom? instance = NCombatRoom.Instance;
            NFireBurstVfx? child = NFireBurstVfx.Create(hittableEnemy, 1.5f);
            instance?.CombatVfxContainer.AddChildSafely(child);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState!)
            .Execute(choiceContext);
    }

    public override Task AfterFlush(PlayerChoiceContext choiceContext, Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        if (!retainedCards.Contains(this))
        {
            return Task.CompletedTask;
        }

        DynamicVars.Damage.BaseValue += DynamicVars[DamageIncreaseKey].BaseValue;
        return Task.CompletedTask;
    }

    public override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}