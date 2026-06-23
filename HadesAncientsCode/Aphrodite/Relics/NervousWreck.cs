using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Relics;

[Pool(typeof(EventRelicPool))]
public class NervousWreck() : HadesAncientsRelic(HadesAncient.Aphrodite)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override Decimal ModifyPowerAmountGivenMultiplicative(
        PowerModel power,
        Creature giver,
        Decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return power.Type != PowerType.Debuff || !Owner.Creature.CombatState!.Enemies.Contains(target) ||
               giver != Owner.Creature
            ? 1
            : 2;
    }

    public override Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        Flash();
        return Task.CompletedTask;
    }
}