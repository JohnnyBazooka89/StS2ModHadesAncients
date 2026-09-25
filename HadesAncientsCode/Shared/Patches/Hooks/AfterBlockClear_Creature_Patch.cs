using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(Creature), "ClearBlock")]
public class AfterBlockClear_Creature_Patch
{
    [HarmonyPrefix]
    public static void Prefix(
        Creature __instance,
        out int __state
    )
    {
        // Capture Block before the original method executes.
        __state = __instance.Block;
    }

    [HarmonyPostfix]
    public static void Postfix(
        Creature __instance,
        int __state,
        ref Task __result
    )
    {
        __result = PostfixAsync(
            __instance,
            __state,
            __result
        );
    }

    private static async Task PostfixAsync(
        Creature creature,
        int blockBeforeClearing,
        Task originalTask
    )
    {
        await originalTask;

        await HadesAncientsHooks.AfterBlockClear(
            creature.CombatState!.RunState,
            creature.CombatState,
            creature,
            blockBeforeClearing
        );
    }
}