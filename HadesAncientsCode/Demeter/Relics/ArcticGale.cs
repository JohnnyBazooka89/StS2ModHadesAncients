using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class ArcticGale() : HadesAncientsRelic(HadesAncient.Demeter)
{
    private const string OrbSlotsKey = "OrbSlots";
    private const string FrostToChannelKey = "FrostToChannel";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(OrbSlotsKey, 3),
        new IntVar(FrostToChannelKey, 2),
        new DamageVar(5, ValueProp.Unpowered)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromOrb<FrostOrb>()
    ];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState!.TurnNumber > 1)
            return;

        Flash();
        await OrbCmd.AddSlots(Owner, DynamicVars[OrbSlotsKey].IntValue);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner ||
            cardPlay.Card.Type != CardType.Power ||
            !CombatManager.Instance.IsInProgress)
            return;

        Flash();
        for (int i = 0; i < DynamicVars[FrostToChannelKey].IntValue; i++)
            await OrbCmd.Channel<FrostOrb>(choiceContext, Owner);

        await Cmd.Wait(0.25f);

        IReadOnlyList<Creature> hittableEnemies = Owner.Creature.CombatState!.HittableEnemies;
        if (hittableEnemies.Count > 0)
        {
            await CreatureCmd.Damage(
                choiceContext,
                hittableEnemies,
                DynamicVars.Damage.BaseValue,
                ValueProp.Unpowered,
                Owner.Creature);
        }
    }
}