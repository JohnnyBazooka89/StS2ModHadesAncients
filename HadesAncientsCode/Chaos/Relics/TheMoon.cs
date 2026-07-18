using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Chaos.Relics.ChaosTypes;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Chaos.Relics;

[Pool(typeof(EventRelicPool))]
public class TheMoon() : HadesAncientsRelic(HadesAncient.Chaos), IArcanaCardRelic
{
    private const string TurnKey = "Turn";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnKey, 3M),
        new EnergyVar(1),
        new CardsVar(2),
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public int GetCardNumber()
    {
        return 5;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return;
        if (combatState.RoundNumber == DynamicVars[TurnKey].IntValue)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), DynamicVars.Cards.BaseValue, Owner);
        }

        InvokeDisplayAmountChanged();
    }
}