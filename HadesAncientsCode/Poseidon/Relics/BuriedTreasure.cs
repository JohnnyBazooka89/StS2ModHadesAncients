using BaseLib.Hooks;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Poseidon.Relics;

[Pool(typeof(EventRelicPool))]
public class BuriedTreasure() : HadesAncientsRelic(HadesAncient.Poseidon), IHealAmountModifier
{
    private const string MorePercentGoldKey = "MorePercentGold";
    private const string MorePercentHealKey = "MorePercentHeal";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MorePercentGoldKey, 50M),
        new(MorePercentHealKey, 50M),
        new GoldVar(100),
        new HealVar(10),
    ];

    public decimal ModifyHealMultiplicative(Creature creature, decimal amount)
    {
        if (creature.Player != Owner)
        {
            return 1;
        }

        Flash();
        return 1 + DynamicVars[MorePercentHealKey].BaseValue / 100M;
    }

    public override decimal ModifyGoldGained(Player player, decimal amount)
    {
        return player != Owner ? amount : amount * (1 + DynamicVars[MorePercentGoldKey].BaseValue / 100M);
    }

    public override Task AfterModifyingGoldGained(Player player, decimal amount)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}