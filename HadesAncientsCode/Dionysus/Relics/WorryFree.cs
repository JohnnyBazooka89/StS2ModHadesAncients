using BaseLib.Hooks;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class WorryFree() : HadesAncientsRelic(HadesAncient.Dionysus), IHealAmountModifier
{
    private const string MinMaxHpKey = "MinMaxHp";
    private const string MaxMaxHpKey = "MaxMaxHp";
    private const string MinHealingPercentKey = "MinHealingPercent";
    private const string MaxHealingPercentKey = "MaxHealingPercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MinMaxHpKey, 40M),
        new(MaxMaxHpKey, 60M),
        new(MinHealingPercentKey, 50M),
        new(MaxHealingPercentKey, 125M),
    ];

    public decimal ModifyHealMultiplicative(Creature creature, decimal amount)
    {
        if (creature.Player != Owner || RandomHealSuppressor.SuppressRandomHeal)
        {
            return 1;
        }

        Flash();
        int randomHealingPercent = Owner.RunState.Rng.Niche.NextInt(
            DynamicVars[MinHealingPercentKey].IntValue,
            DynamicVars[MaxHealingPercentKey].IntValue + 1
        );
        return randomHealingPercent / 100M;
    }

    public override async Task AfterObtained()
    {
        await RandomHealSuppressor.Run(async () =>
        {
            await CreatureCmd.GainMaxHp(
                Owner.Creature,
                Owner.RunState.Rng.Niche.NextInt(
                    DynamicVars[MinMaxHpKey].IntValue,
                    DynamicVars[MaxMaxHpKey].IntValue + 1
                )
            );
        });
    }

    public static class RandomHealSuppressor
    {
        [field: ThreadStatic] public static bool SuppressRandomHeal { get; private set; }

        public static async Task Run(Func<Task> action)
        {
            bool previous = SuppressRandomHeal;
            SuppressRandomHeal = true;

            try
            {
                await action();
            }
            finally
            {
                SuppressRandomHeal = previous;
            }
        }
    }
}