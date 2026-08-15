using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class MutualDestruction() : HadesAncientsRelic(HadesAncient.Ares), IModifyDamageMultiplicativeCompatibility,
    ICustomRelicStringLabel
{
    private const string HpThresholdLowHpKey = "HpThresholdLowHp";
    private const string HpThresholdMediumHpKey = "HpThresholdMediumHp";
    private const string HpThresholdHighHpKey = "HpThresholdHighHp";

    private const string MoreDamageLowHpKey = "MoreDamageLowHp";
    private const string MoreDamageMediumHpKey = "MoreDamageMediumHp";
    private const string MoreDamageHighHpKey = "MoreDamageHighHp";

    private static readonly (string ThresholdKey, string DamageKey)[] DamageTiers =
    [
        (HpThresholdLowHpKey, MoreDamageLowHpKey),
        (HpThresholdMediumHpKey, MoreDamageMediumHpKey),
        (HpThresholdHighHpKey, MoreDamageHighHpKey)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HpThresholdLowHpKey, 25M),
        new(HpThresholdMediumHpKey, 50M),
        new(HpThresholdHighHpKey, 75M),
        new(MoreDamageLowHpKey, 100M),
        new(MoreDamageMediumHpKey, 50M),
        new(MoreDamageHighHpKey, 25M)
    ];

    public decimal ModifyDamageMultiplicativeCompatibility(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || (dealer != Owner.Creature && dealer != Owner.Osty))
            return 1M;

        return 1M + GetMoreDamagePercent() / 100M;
    }

    public bool ShowCustomStringDisplayLabel => CombatManager.Instance.IsInProgress;

    public string CustomStringDisplayLabel => $"+{GetMoreDamagePercent()}%";

    private decimal GetMoreDamagePercent()
    {
        foreach (var (thresholdKey, damageKey) in DamageTiers)
        {
            var threshold = Owner.Creature.MaxHp * (DynamicVars[thresholdKey].BaseValue / 100M);

            if (Owner.Creature.CurrentHp <= threshold)
                return DynamicVars[damageKey].BaseValue;
        }

        return 0M;
    }

    public override async Task AfterObtained()
    {
        await SetActiveIfNecessary();
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        await SetActiveIfNecessary();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal _)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;
        await SetActiveIfNecessary();
    }

    private Task SetActiveIfNecessary()
    {
        Creature creature = Owner.Creature;
        bool flag = creature.CurrentHp <=
                    creature.MaxHp * (DynamicVars[HpThresholdHighHpKey].BaseValue / 100M);
        Status = flag ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}