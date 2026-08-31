using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
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

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class FlameStrike() : HadesAncientsRelic(HadesAncient.Hestia)
{
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
        new CardsVar(3)
    ];

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return !CombatManager.Instance.IsInProgress
               || Charges <= 0
               || card.Type != CardType.Attack
               || card.Owner.Creature != Owner.Creature
            ? playCount
            : playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
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
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars.Cards.IntValue;
        return Task.CompletedTask;
    }
}