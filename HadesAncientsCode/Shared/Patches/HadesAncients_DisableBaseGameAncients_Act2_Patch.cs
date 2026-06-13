using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(Hive), nameof(Hive.AllAncients), MethodType.Getter)]
public class HadesAncients_DisableBaseGameAncients_Act2_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref IEnumerable<AncientEventModel> __result)
    {
        if (!HadesAncientsModConfig.DisableBaseGameAncients)
        {
            return;
        }

        __result = __result
            .Where(ancient =>
                ancient is not Orobas &&
                ancient is not Pael &&
                ancient is not Tezcatara)
            .ToArray();
    }
}