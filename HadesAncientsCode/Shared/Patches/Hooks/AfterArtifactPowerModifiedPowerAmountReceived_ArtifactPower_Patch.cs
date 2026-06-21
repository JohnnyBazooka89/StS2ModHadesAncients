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
        __result = PostfixAsync(__instance, power, __result);
    }

    private static async Task PostfixAsync(ArtifactPower artifactPower, PowerModel power, Task originalTask)
    {
        await originalTask;
        await HadesAncientsHooks.AfterArtifactPowerModifiedPowerAmountReceived(
            artifactPower.Owner.CombatState?.RunState,
            artifactPower.Owner.CombatState,
            artifactPower,
            power
        );
    }
}