using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

public class HadesAncients_SetNeowInCustomRuns_Patches
{
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]
    public static class HadesAncients_RunManager_GenerateRooms_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(RunManager __instance)
        {
            if (__instance.State is { Modifiers.Count: > 0, Acts.Count: > 0 } &&
                __instance.State.Acts[0]._rooms._ancient is not Neow)
            {
                __instance.State.Acts[0]._rooms._ancient = ModelDb.Event<Neow>();
            }
        }
    }

    [HarmonyPatch(typeof(RoomSet), nameof(RoomSet.Ancient), MethodType.Getter)]
    public static class HadesAncients_RoomSet_Ancient_Getter_Patch
    {
        [HarmonyPostfix]
        private static void Prefix(RoomSet __instance)
        {
            RunState? state = RunManager.Instance.DebugOnlyGetState();
            if (state == null)
            {
                return;
            }

            if (state is { Modifiers.Count: > 0, Acts.Count: > 0 } &&
                state.Acts[0]._rooms._ancient is not Neow)
            {
                state.Acts[0]._rooms._ancient = ModelDb.Event<Neow>();
            }
        }
    }
}