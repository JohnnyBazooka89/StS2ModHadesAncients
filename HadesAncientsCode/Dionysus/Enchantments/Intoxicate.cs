using HadesAncients.HadesAncientsCode.Dionysus.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Dionysus.Enchantments;

public class Intoxicate() : HadesAncientsEnchantment(HadesAncient.Dionysus)
{
    [SavedProperty]
    private int IntoxicateMarker { get; set; }
    
    public override bool HasExtraCardText => true;
    public override bool ShowAmount => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<HangoverPower>()
    ];

    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Skill;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != Card || !CombatManager.Instance.IsInProgress)
        {
            return;
        }

        IEnumerable<Creature> targets = Card.Owner.Creature.CombatState?.GetOpponentsOf(Card.Owner.Creature)
            .Where(c => c.IsAlive) ?? [];
        await PowerCmd.Apply<HangoverPower>(choiceContext, targets,
            Amount, Card.Owner.Creature, Card);
    }
}