using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class HighlyFlammable() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string MoreStacksKey = "MoreStacks";

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MoreStacksKey, 2)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override decimal ModifyPowerAmountGivenAdditive(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return power.Type != PowerType.Debuff || !Owner.Creature.CombatState!.Enemies.Contains(target) ||
               giver != Owner.Creature
            ? 0
            : 2;
    }

    public override Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        Flash();
        return Task.CompletedTask;
    }
}