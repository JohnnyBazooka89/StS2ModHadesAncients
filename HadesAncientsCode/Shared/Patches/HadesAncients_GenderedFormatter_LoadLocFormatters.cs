using HadesAncients.HadesAncientsCode.Shared.Formatters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(LocManager), "LoadLocFormatters")]
public static class HadesAncients_GenderedFormatter_LoadLocFormatters
{
    private static void Postfix()
    {
        LocManager._smartFormatter.AddExtensions(new GenderedFormatter());
    }
}