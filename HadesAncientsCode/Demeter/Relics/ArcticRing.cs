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
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class ArcticRing() : HadesAncientsRelic(HadesAncient.Demeter)
{
    private int _skillsPlayedThisTurn;
    private int _damageToDeal;
    private bool _isActivating;
    private bool _usedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => DamageToDeal;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DamageVar(3, ValueProp.Unpowered),
        new PowerVar<WeakPower>(1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    private int SkillsPlayedThisTurn
    {
        get => _skillsPlayedThisTurn;
        set
        {
            AssertMutable();
            _skillsPlayedThisTurn = value;
            UpdateDisplay();
        }
    }

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

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            Status = !_usedThisTurn &&
                     SkillsPlayedThisTurn == DynamicVars.Cards.IntValue - 1
                ? RelicStatus.Active
                : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Skill)
            return;

        SkillsPlayedThisTurn++;
        DamageToDeal += DynamicVars.Damage.IntValue;

        if (_usedThisTurn || SkillsPlayedThisTurn < DynamicVars.Cards.IntValue)
            return;

        _ = TaskHelper.RunSafely(DoActivateVisuals());
        await PowerCmd.Apply<WeakPower>(choiceContext, Owner.Creature.CombatState!.HittableEnemies,
            DynamicVars.Weak.BaseValue, Owner.Creature, null);
        _usedThisTurn = true;
        UpdateDisplay();
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature) || DamageToDeal <= 0)
            return;

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

        SkillsPlayedThisTurn = 0;
        DamageToDeal = 0;
        _usedThisTurn = false;
        UpdateDisplay();
        return Task.CompletedTask;
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
        SkillsPlayedThisTurn = 0;
        DamageToDeal = 0;
        _usedThisTurn = false;
        IsActivating = false;
        UpdateDisplay();
        return Task.CompletedTask;
    }
}