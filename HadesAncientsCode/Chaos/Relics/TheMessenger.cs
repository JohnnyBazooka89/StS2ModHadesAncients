using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Chaos.Relics.Types;
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

namespace HadesAncients.HadesAncientsCode.Chaos.Relics;

[Pool(typeof(EventRelicPool))]
public class TheMessenger() : HadesAncientsRelic(HadesAncient.Chaos), IArcanaRelic
{
    private bool _isActivating;
    private int _skillsPlayedThisTurn;
    private bool UsedThisCombat { get; set; }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress && (IsActivating || !UsedThisCombat);

    public override int DisplayAmount =>
        !IsActivating ? SkillsPlayedThisTurn % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new PowerVar<BufferPower>(1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BufferPower>()
    ];

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

    public int GetArcanaRelicNumber()
    {
        return 8;
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            int intValue = DynamicVars.Cards.IntValue;
            Status = SkillsPlayedThisTurn % intValue == intValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task BeforeCombatStart()
    {
        SkillsPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        UsedThisCombat = false;
        UpdateDisplay();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState!.TurnNumber == 1)
            return Task.CompletedTask;
        SkillsPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Skill || UsedThisCombat)
            return;
        SkillsPlayedThisTurn++;
        int intValue = DynamicVars.Cards.IntValue;
        if (SkillsPlayedThisTurn % intValue != 0)
            return;
        _ = TaskHelper.RunSafely(DoActivateVisuals());

        await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(BufferPower)].BaseValue, Owner.Creature, null);
        UsedThisCombat = true;
        UpdateDisplay();
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
        IsActivating = false;
        UsedThisCombat = false;
        UpdateDisplay();
        return Task.CompletedTask;
    }
}