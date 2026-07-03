using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Relics;

[Pool(typeof(EventRelicPool))]
public class ShamelessAttitude() : HadesAncientsRelic(HadesAncient.Aphrodite)
{
    private const string HpThresholdKey = "HpThreshold";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HpThresholdKey, 50M),
        new EnergyVar(1),
        new CardsVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        await SetActiveIfNecessary();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal _)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;
        await SetActiveIfNecessary();
    }

    private async Task SetActiveIfNecessary()
    {
        Creature creature = Owner.Creature;
        bool flag = creature.CurrentHp >=
                    creature.MaxHp * (DynamicVars[HpThresholdKey].BaseValue / 100M);
        Status = flag ? RelicStatus.Active : RelicStatus.Normal;
    }

    public override Decimal ModifyDamageMultiplicative(Creature? target, Decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null || (dealer != Owner.Creature && dealer != Owner.Osty))
            return 1M;

        return Status == RelicStatus.Active ? 5M / 3M : 4M / 3M;
    }
}