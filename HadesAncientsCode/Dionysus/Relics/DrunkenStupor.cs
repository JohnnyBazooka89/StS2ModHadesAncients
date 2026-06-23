using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Dionysus.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class DrunkenStupor() : HadesAncientsRelic(HadesAncient.Dionysus)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<HangoverPower>(8M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<HangoverPower>()
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Power)
        {
            return;
        }

        IEnumerable<Creature> targets = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive);
        Flash();
        await PowerCmd.Apply<HangoverPower>(choiceContext, targets,
            DynamicVars["HangoverPower"].BaseValue, Owner.Creature, null);
    }
}