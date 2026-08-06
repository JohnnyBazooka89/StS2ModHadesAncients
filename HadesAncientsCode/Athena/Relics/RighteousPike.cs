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
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class RighteousPike() : HadesAncientsRelic(HadesAncient.Athena)
{
    private int _energySpent;
    private bool _isActivating;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? EnergySpent : DynamicVars.Energy.IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(3),
        new DamageVar(4, ValueProp.Unpowered),
        new RepeatVar(3)
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
    private int EnergySpent
    {
        get => _energySpent;
        set
        {
            AssertMutable();
            _energySpent = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.IsAutoPlay)
        {
            return;
        }

        EnergySpent += cardPlay.Resources.EnergySpent;
        while (EnergySpent >= DynamicVars.Energy.IntValue)
        {
            _ = TaskHelper.RunSafely(DoActivateVisuals());

            for (int i = 0; i < DynamicVars.Repeat.BaseValue; i++)
            {
                await DealSpearDamage(DynamicVars.Damage.BaseValue, new ThrowingPlayerChoiceContext());
            }

            EnergySpent -= DynamicVars.Energy.IntValue;
        }

        Status = EnergySpent == DynamicVars.Energy.IntValue - 1
            ? RelicStatus.Active
            : RelicStatus.Normal;
    }

    private async Task DealSpearDamage(
        Decimal value,
        PlayerChoiceContext choiceContext)
    {
        List<Creature> hittableEnemies = Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature)
            .Where(e => e.IsHittable).ToList() ?? [];
        if (hittableEnemies.Count == 0)
            return;
        Creature? target = Owner.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
        if (target == null)
            return;
        await Cmd.Wait(0.25f);
        VfxCmd.PlayOnCreature(target, VfxCmd.slashPath);
        await CreatureCmd.Damage(choiceContext, target, value, ValueProp.Unpowered, Owner.Creature);
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