using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Unlocks;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(
    typeof(ActModel),
    nameof(ActModel.GenerateRooms), typeof(Rng), typeof(UnlockState), typeof(bool))]
public class HadesAncients_DisableBaseGameAncients_SharedAncients_GenerateRooms_Patch
{
    private static readonly AccessTools.FieldRef<ActModel, List<AncientEventModel>> SharedAncientSubsetRef =
        AccessTools.FieldRefAccess<ActModel, List<AncientEventModel>>("_sharedAncientSubset");

    [HarmonyPrefix]
    public static void Prefix(ActModel __instance)
    {
        if (!HadesAncientsModConfig.DisableBaseGameAncients)
            return;

        List<AncientEventModel> sharedAncientSubset = SharedAncientSubsetRef(__instance);

        sharedAncientSubset?.RemoveAll(ancient => ancient is Darv);
    }
}