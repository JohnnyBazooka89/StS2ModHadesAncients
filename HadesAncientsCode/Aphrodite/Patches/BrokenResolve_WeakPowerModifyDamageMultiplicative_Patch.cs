using HadesAncients.HadesAncientsCode.Aphrodite.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Patches;

[HarmonyPatch(typeof(WeakPower), nameof(WeakPower.ModifyDamageMultiplicative))]
public static class BrokenResolve_WeakPowerModifyDamageMultiplicative_Patch
{
    public static void Postfix(
        WeakPower __instance,
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        ref decimal __result)
    {
        BrokenResolve? brokenResolve = target?.Player?.GetRelic<BrokenResolve>();
        if (brokenResolve == null || __result is < 0.5M or > 1M)
        {
            return;
        }

        decimal newValue =
            dealer != __instance.Owner || !props.IsPoweredAttack()
                ? __result
                : __result - (1M - __result);

        __result = Math.Max(0.5M, newValue);
    }
}