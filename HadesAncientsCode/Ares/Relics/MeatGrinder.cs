using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class MeatGrinder() : HadesAncientsRelic(HadesAncient.Ares)
{
    private const string EnergyThresholdKey = "EnergyThreshold";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(EnergyThresholdKey, 2),
        new DamageVar(8, ValueProp.Unpowered),
        new RepeatVar(2)
    ];
    
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Resources.EnergySpent < DynamicVars[EnergyThresholdKey].IntValue)
            return;
        Flash();
        for (int i = 0; i < DynamicVars.Repeat.BaseValue; i++)
        {
            await Cmd.Wait(0.25f);
            VfxCmd.PlayOnCreatures(Owner.Creature.CombatState!.HittableEnemies, VfxCmd.bluntPath);
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState!.HittableEnemies,
                DynamicVars.Damage.IntValue, ValueProp.Unpowered, Owner.Creature);
        }
    }
}