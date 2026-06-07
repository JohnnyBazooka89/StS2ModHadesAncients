using Godot;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(ModManager), nameof(ModManager.GetModdedLocTables))]
public static class HadesAncients_LoadLocalizations_Patch
{
    public static void Postfix(
        string language,
        string file,
        ref IEnumerable<string> __result)
    {
        __result = AddHadesAncientsLocTables(__result, language, file);
    }

    private static IEnumerable<string> AddHadesAncientsLocTables(
        IEnumerable<string> originalResult,
        string language,
        string file)
    {
        foreach (string path in originalResult)
            yield return path;

        foreach (Mod mod in ModManager._mods)
        {
            if (mod.state != ModLoadState.Loaded || mod.manifest?.id != "HadesAncients")
                continue;

            foreach (HadesAncient ancient in Enum.GetValues<HadesAncient>())
            {
                string extraPath =
                    $"res://{mod.manifest.id}/{ancient.ToString()}/localization/{language}/{file}";

                if (ResourceLoader.Exists(extraPath))
                    yield return extraPath;
            }

            string sharedPath =
                $"res://{mod.manifest.id}/Shared/localization/{language}/{file}";

            if (ResourceLoader.Exists(sharedPath))
                yield return sharedPath;
        }
    }
}