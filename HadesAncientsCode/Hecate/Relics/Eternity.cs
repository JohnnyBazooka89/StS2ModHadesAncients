using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Powers;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Eternity() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string StrengthLossKey = "StrengthLoss";
    private const string EnergyThresholdKey = "EnergyThreshold";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool UsedThisCombat { get; set; }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Unpowered),
        new EnergyVar(EnergyThresholdKey, 2),
        new(StrengthLossKey, 4M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public int GetArcanaRelicNumber()
    {
        return 4;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner ||
            cardPlay.Resources.EnergyValue < DynamicVars[EnergyThresholdKey].IntValue ||
            UsedThisCombat)
            return;
        Flash();
        UsedThisCombat = true;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (Creature hittableEnemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<EternityPower>(context, hittableEnemy, DynamicVars[StrengthLossKey].BaseValue,
                Owner.Creature, null);
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return Task.CompletedTask;
        UsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisCombat = false;
        return Task.CompletedTask;
    }
}