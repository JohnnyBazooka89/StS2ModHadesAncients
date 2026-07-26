using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]
public class HadesAncients_RunManager_GenerateRooms_Patch
{
    [HarmonyPostfix]
    private static void Postfix(RunManager __instance)
    {
        if (__instance.State is { Modifiers.Count: > 0, Acts.Count: > 0 } &&
            __instance.State.Acts[0].Ancient is not Neow)
        {
            __instance.State.Acts[0]._rooms.Ancient = ModelDb.Event<Neow>();
        }
    }
}