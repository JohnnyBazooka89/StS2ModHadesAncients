using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Aphrodite.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Relics;

[Pool(typeof(EventRelicPool))]
public class SweetSurrender() : HadesAncientsRelic(HadesAncient.Aphrodite)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CharmPower>(),
    ];

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0M || applier != Owner.Creature || !Owner.Creature.CombatState.Enemies.Contains(power.Owner) ||
            power.Id != ModelDb.GetId<WeakPower>())
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<VulnerablePower>(choiceContext, power.Owner, amount, Owner.Creature, null);
        await PowerCmd.Apply<CharmPower>(choiceContext, power.Owner, amount, Owner.Creature, null);
    }
}