using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]
public class CombatRoomOfferRoomEndRewardsPatch
{
    [HarmonyPostfix]
    private static void Postfix(
        CombatRoom __instance,
        ref Task __result
    )
    {
        __result = RunAfterOriginal(__instance, __result);
    }

    private static async Task RunAfterOriginal(
        CombatRoom room,
        Task originalTask
    )
    {
        // Preserve the original exception behavior. The custom hook is only
        // invoked when OfferRoomEndRewards completes successfully.
        await originalTask;

        await HadesAncientsHooks.AfterOfferRoomEndRewards(
            room.CombatState.RunState,
            room.CombatState,
            room
        );
    }
}