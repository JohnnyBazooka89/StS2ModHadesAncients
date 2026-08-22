using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class PremiumService() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    private readonly List<CardModel> _deferredOpeningCards = [];
    private int _charges;

    // Only needed during the initial hand draw.
    private bool _deferOpeningHand;

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

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    public override int DisplayAmount => Charges;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(10)
    ];

    public override Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1)
            return Task.CompletedTask;

        Charges = DynamicVars.Cards.IntValue;

        _deferredOpeningCards.Clear();
        _deferOpeningHand = true;

        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner != Owner ||
            card.Owner.Creature.CombatState!.CurrentSide != card.Owner.Creature.Side ||
            Charges <= 0)
        {
            return Task.CompletedTask;
        }

        // The opening hand needs to wait until Bellows has resolved.
        //
        // Store the cards in draw order so that Premium Service still
        // correctly affects the "first" eligible cards.
        if (_deferOpeningHand && fromHandDraw)
        {
            _deferredOpeningCards.Add(card);
            return Task.CompletedTask;
        }

        UpgradeIfPossible(card);

        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStartLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner || !_deferOpeningHand)
            return Task.CompletedTask;

        _deferOpeningHand = false;

        foreach (CardModel card in _deferredOpeningCards)
        {
            UpgradeIfPossible(card);
        }

        _deferredOpeningCards.Clear();

        return Task.CompletedTask;
    }

    private void UpgradeIfPossible(CardModel card)
    {
        if (Charges <= 0 || !card.IsUpgradable)
            return;

        CardCmd.Upgrade(card);
        Charges--;
        Flash();
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars.Cards.IntValue;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _deferOpeningHand = false;
        _deferredOpeningCards.Clear();

        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}