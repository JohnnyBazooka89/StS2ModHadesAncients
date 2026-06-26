using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.FinalizeUpgradeInternal))]
public static class AfterCardUpgrade_FinalizeUpgradeInternal_Patch
{
    public static void Postfix(CardModel __instance)
    {
        _ = TaskHelper.RunSafely(HadesAncientsHooks.AfterCardUpgrade(__instance.Owner?.RunState,
            __instance.Owner?.Creature.CombatState, __instance));
    }
}