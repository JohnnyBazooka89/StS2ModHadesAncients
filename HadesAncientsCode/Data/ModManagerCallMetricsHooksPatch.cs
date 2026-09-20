using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves;

namespace HadesAncients.HadesAncientsCode.Data;

/**
 * There is a problem with ModManager.OnMetricsUpload += HadesAncientsMetrics.OnMetricsUpload;
 * because of Publicizer, so this patch is a workaround.
 */
[HarmonyPatch(typeof(ModManager), nameof(ModManager.CallMetricsHooks))]
public static class ModManagerCallMetricsHooksPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        SerializableRun run,
        bool isVictory,
        ulong localPlayerId)
    {
        HadesAncientsMetrics.OnMetricsUpload(
            run,
            isVictory,
            localPlayerId
        );
    }
}