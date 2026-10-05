using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Artemis.Vfx;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class SupportFire() : HadesAncientsRelic(HadesAncient.Artemis)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2, ValueProp.Unpowered)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;

        List<Creature> targets = Owner.Creature.CombatState!.HittableEnemies.ToList();

        if (targets.Count < 1)
        {
            return;
        }

        Owner.RunState.Rng.CombatTargets.Shuffle(targets);
        NArrowVfx vfx = PreloadManager.Cache
            .GetScene(NArrowVfx.scenePath)
            .Instantiate<NArrowVfx>();

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);

        Flash();
        await NArrowVfx.Create(Owner.Creature, targets[0]);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), targets[0], DynamicVars.Damage,
            Owner.Creature);
    }
}