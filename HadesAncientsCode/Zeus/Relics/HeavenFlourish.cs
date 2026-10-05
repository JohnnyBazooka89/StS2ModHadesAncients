using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Zeus.Relics;

[Pool(typeof(EventRelicPool))]
public class HeavenFlourish() : HadesAncientsRelic(HadesAncient.Zeus), ICardPlayResultLocationCompatibility
{
    private int _charges;
    private CardModel? _pendingCardToActivate;

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
        new CardsVar(3)
    ];

    public CardLocationCompatibility ModifyCardPlayResultLocationCompatibility(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocationCompatibility cardLocation)
    {
        _pendingCardToActivate = null;

        if (!isAutoPlay
            && CombatManager.Instance.IsInProgress
            && Charges > 0
            && card.Owner == Owner
            && card.Type == CardType.Skill)
        {
            _pendingCardToActivate = card;
        }

        return cardLocation;
    }

    public override int ModifyCardPlayCount(
        CardModel card,
        Creature? target,
        int playCount)
    {
        if (!ReferenceEquals(card, _pendingCardToActivate))
            return playCount;

        return playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        Charges--;
        return Task.CompletedTask;
    }

    public override Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1)
        {
            return Task.CompletedTask;
        }

        Charges = DynamicVars.Cards.IntValue;
        _pendingCardToActivate = null;
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars.Cards.IntValue;
        _pendingCardToActivate = null;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Charges = 0;
        _pendingCardToActivate = null;
        return Task.CompletedTask;
    }
}