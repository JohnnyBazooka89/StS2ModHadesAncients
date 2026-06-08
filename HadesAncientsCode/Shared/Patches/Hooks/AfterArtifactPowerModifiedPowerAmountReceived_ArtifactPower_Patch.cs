using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(ArtifactPower), nameof(ArtifactPower.AfterModifyingPowerAmountReceived))]
public static class AfterArtifactPowerModifiedPowerAmountReceived_ArtifactPower_Patch
{
    public static void Postfix(
        ArtifactPower __instance,
        PowerModel power,
        ref Task __result
    )
    {
        __result = HadesAncientsHooks.AfterArtifactPowerModifiedPowerAmountReceived(
            __result,
            __instance.Owner.CombatState?.RunState,
            __instance.Owner.CombatState,
            __instance,
            power
        );
    }
}