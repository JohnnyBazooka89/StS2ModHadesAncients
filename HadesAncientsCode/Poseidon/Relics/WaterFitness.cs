using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HadesAncients.HadesAncientsCode.Shared.Vars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Poseidon.Relics;

[Pool(typeof(EventRelicPool))]
public class WaterFitness() : HadesAncientsRelic(HadesAncient.Poseidon), IAfterAnyRelicObtained
{
    private const string TotalHpToGainKey = "TotalHpToGain";

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(2M),
        new(TotalHpToGainKey + "Base", 0M),
        new(TotalHpToGainKey + "Extra", 1M),
        new OutsideCombatCalculatedVar(TotalHpToGainKey).WithMultiplier(static (relic, _) =>
            (relic.Owner.Relics.Count + 1) * relic.DynamicVars.MaxHp.BaseValue)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public async Task AfterAnyRelicObtained(Player player, RelicModel relic)
    {
        if (relic == this)
        {
            return;
        }

        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }

    public override async Task AfterObtained()
    {
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature,
            ((OutsideCombatCalculatedVar)DynamicVars[TotalHpToGainKey]).CalculateCustom(null) -
            DynamicVars.MaxHp.BaseValue);
    }
}