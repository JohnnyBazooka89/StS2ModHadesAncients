using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class GrislyGain() : HadesAncientsRelic(HadesAncient.Ares)
{
    private int _attacksPlayed;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => AttacksPlayed;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4)
    ];

    [SavedProperty]
    private int AttacksPlayed
    {
        get => _attacksPlayed;
        set
        {
            AssertMutable();
            _attacksPlayed = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        Status = AttacksPlayed == DynamicVars.Cards.IntValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldModifyCost(card))
            return false;
        modifiedCost = 0M;
        return true;
    }

    public override bool TryModifyStarCost(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldModifyCost(card))
            return false;
        modifiedCost = 0M;
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress || cardPlay.IsAutoPlay || !cardPlay.IsFirstInSeries ||
            cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;
        if (cardPlay.Card.Type == CardType.Attack)
        {
            ++AttacksPlayed;
        }

        AttacksPlayed %= DynamicVars.Cards.IntValue;

        return Task.CompletedTask;
    }

    private bool ShouldModifyCost(CardModel card)
    {
        if (!CombatManager.Instance.IsInProgress || card.Owner.Creature != Owner.Creature ||
            AttacksPlayed != DynamicVars.Cards.BaseValue - 1M || card.Type != CardType.Attack)
            return false;
        PileType? type = card.Pile?.Type;
        if (type.HasValue)
        {
            return type.GetValueOrDefault() switch
            {
                PileType.Hand or PileType.Play => true,
                _ => false
            };
        }

        return false;
    }
}