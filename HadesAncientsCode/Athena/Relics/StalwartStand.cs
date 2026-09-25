using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Athena.Enums;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class StalwartStand() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string DeathDefianceToGainKey = "DeathDefianceToGain";
    private const string HpThresholdKey = "HpThreshold";
    private int _deathDefiances;
    private bool _dexterityApplied;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => DeathDefiances;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(DeathDefianceToGainKey, 1M),
        new(HpThresholdKey, 40M),
        new HealVar(40M),
        new PowerVar<DexterityPower>(2M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(AthenaStaticHoverTips.DeathDefiance),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    private bool DexterityApplied
    {
        get => _dexterityApplied;
        set
        {
            AssertMutable();
            _dexterityApplied = value;
        }
    }

    [SavedProperty]
    private int DeathDefiances
    {
        get => _deathDefiances;
        set
        {
            AssertMutable();
            _deathDefiances = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShouldDieLate(Creature creature)
    {
        return creature != Owner.Creature || DeathDefiances <= 0;
    }

    public override Task AfterObtained()
    {
        DeathDefiances = DynamicVars[DeathDefianceToGainKey].IntValue;
        return Task.CompletedTask;
    }

    public override Task AfterActEntered()
    {
        DeathDefiances += DynamicVars[DeathDefianceToGainKey].IntValue;
        return Task.CompletedTask;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        DeathDefiances--;
        await CreatureCmd.Heal(creature, Math.Max(1M, creature.MaxHp * (DynamicVars.Heal.BaseValue / 100M)));
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        await ModifyDexterityIfNecessary();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        DexterityApplied = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal _)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;
        await ModifyDexterityIfNecessary();
    }

    private async Task ModifyDexterityIfNecessary()
    {
        Creature creature = Owner.Creature;
        bool flag = creature.CurrentHp >
                    creature.MaxHp * (DynamicVars[HpThresholdKey].BaseValue / 100M);
        Status = flag ? RelicStatus.Normal : RelicStatus.Active;
        decimal dexterityBaseValue = DynamicVars.Dexterity.BaseValue;
        if (flag && DexterityApplied)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), creature, -dexterityBaseValue,
                creature, null);
            DexterityApplied = false;
        }
        else
        {
            if (flag || DexterityApplied)
                return;
            Flash();
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), creature, dexterityBaseValue,
                creature, null);
            DexterityApplied = true;
        }
    }
}