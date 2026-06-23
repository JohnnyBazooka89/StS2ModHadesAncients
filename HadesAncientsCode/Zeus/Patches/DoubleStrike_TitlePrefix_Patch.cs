using HadesAncients.HadesAncientsCode.Zeus.Enchantments;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Zeus.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Title), MethodType.Getter)]
public class DoubleStrike_TitlePrefix_Patch
{
    private static readonly LocString titlePrefix =
        LocString.GetIfExists(EnchantmentModel.locTable, $"{ModelDb.GetId<DoubleEnchantment>().Entry}.titlePrefix")!;

    public static void Postfix(CardModel __instance, ref string __result)
    {
        if (__instance.Enchantment is DoubleEnchantment)
        {
            __result = titlePrefix.GetFormattedText() + __result;
        }
    }
}