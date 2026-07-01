using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hephaestus.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class VolcanicFlourish() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlastPower>(3M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BlastPower>()
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Skill || cardPlay.Card.Owner != Owner)
            return;

        List<Creature> targets = Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive).ToList();

        if (targets.Count >= 1)
        {
            Owner.RunState.Rng.CombatTargets.Shuffle(targets);
            await PowerCmd.Apply<BlastPower>(choiceContext, targets[0], DynamicVars[nameof(BlastPower)].BaseValue,
                Owner.Creature, null);
            Flash();
        }
    }
}