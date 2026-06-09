using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Athena.Enums;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class StalwartStand() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string DeathDefianceToGainKey = "DeathDefianceToGain";
    private int _deathDefiances;


    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => DeathDefiances;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(DeathDefianceToGainKey, 1M),
        new HealVar(40M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTips.DeathDefiance)
    ];

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
}