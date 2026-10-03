using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class FullyLoaded() : HadesAncientsRelic(HadesAncient.Artemis)
{
    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState!.TurnNumber > 1)
            return;
        IReadOnlyList<CardModel> possibleAttacks = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == CardType.Attack).ToList();
        if (possibleAttacks.Count == 0)
            return;
        Flash();
        List<CardModel> attacksToAdd = CardFactory
            .GetDistinctForCombat(Owner, possibleAttacks, DynamicVars.Cards.IntValue,
                Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (CardModel cardModel in attacksToAdd)
            cardModel.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardsToCombat(attacksToAdd, PileType.Hand, Owner);
    }
}