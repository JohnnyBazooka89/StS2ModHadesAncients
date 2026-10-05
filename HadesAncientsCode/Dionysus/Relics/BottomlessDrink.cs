using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class BottomlessDrink() : HadesAncientsRelic(HadesAncient.Dionysus)
{
    private const string TurnsKey = "Turns";

    private int _charges;

    private int Charges
    {
        get => _charges;
        set
        {
            AssertMutable();
            _charges = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => Charges;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnsKey, 5)
    ];

    public override Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1)
        {
            return Task.CompletedTask;
        }

        Charges = DynamicVars[TurnsKey].IntValue;
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars[TurnsKey].IntValue;
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Charges <= 0)
            return;

        IReadOnlyList<CardModel> possiblePowers = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == CardType.Power)
            .ToList();

        if (possiblePowers.Count == 0)
            return;

        Flash();

        List<CardModel> powersToAdd = CardFactory
            .GetDistinctForCombat(
                Owner,
                possiblePowers,
                1,
                Owner.RunState.Rng.CombatCardGeneration)
            .ToList();

        foreach (CardModel cardModel in powersToAdd)
            cardModel.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardsToCombat(
            powersToAdd,
            PileType.Hand,
            Owner);

        Charges--;
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}