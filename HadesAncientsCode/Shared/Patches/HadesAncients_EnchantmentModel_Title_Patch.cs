using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(EnchantmentModel), nameof(EnchantmentModel.Title), MethodType.Getter)]
public static class HadesAncients_EnchantmentModel_Title_Patch
{
    [HarmonyPostfix]
    public static void Postfix(
        EnchantmentModel __instance,
        ref LocString __result)
    {
        if (__instance is not HadesAncientsEnchantment)
            return;

        __result.Add("Amount", __instance.Amount);
    }
}