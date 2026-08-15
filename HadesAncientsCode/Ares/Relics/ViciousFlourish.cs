using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Ares.Powers;
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

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class ViciousFlourish() : HadesAncientsRelic(HadesAncient.Ares)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WoundsPower>(4M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WoundsPower>()
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Skill)
        {
            return;
        }

        List<Creature> targets = Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive).ToList();

        if (targets.Count <= 0)
        {
            return;
        }

        Owner.RunState.Rng.CombatTargets.Shuffle(targets);
        Flash();
        await PowerCmd.Apply<WoundsPower>(choiceContext, targets[0], DynamicVars[nameof(WoundsPower)].BaseValue,
            Owner.Creature, null);
    }
}