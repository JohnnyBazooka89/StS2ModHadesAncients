using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(NGame), nameof(NGame.LoadRun))]
public static class AfterLoadRun_LoadRun_Patch
{
    public static void Postfix(
        RunState runState,
        SerializableRoom? preFinishedRoom,
        ref Task __result
    )
    {
        __result = PostfixAsync(__result, runState, preFinishedRoom);
    }

    private static async Task PostfixAsync(
        Task original,
        RunState runState,
        SerializableRoom? preFinishedRoom
    )
    {
        await original;

        await HadesAncientsHooks.AfterLoadRun(
            runState,
            preFinishedRoom
        );
    }
}