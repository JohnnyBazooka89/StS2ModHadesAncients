using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(AncientDialogueSet), nameof(AncientDialogueSet.GetValidDialogues))]
public static class HadesAncients_AllowAllRepeatingDialogues_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(
        AncientDialogueSet __instance,
        ModelId characterId,
        int charVisits,
        int totalVisits,
        bool allowAnyCharacterDialogues,
        ref IEnumerable<AncientDialogue> __result)
    {
        // Preserve vanilla FirstVisitEverDialogue behavior.
        if (totalVisits == 0)
            return true;

        // Determine whether this dialogue set is ours.
        AncientDialogue? firstDialogue = __instance.AgnosticDialogues.FirstOrDefault();

        if (firstDialogue?.Lines.FirstOrDefault()?.LineText?.LocEntryKey.StartsWith("HADESANCIENTS") != true)
        {
            // Not ours -> run original method.
            return true;
        }

        // Ours -> ignore VisitIndex completely.
        var result = new List<AncientDialogue>();

        __instance.CharacterDialogues.TryGetValue(
            characterId.Entry,
            out IReadOnlyList<AncientDialogue>? characterDialogues);

        if (characterDialogues != null)
            result.AddRange(characterDialogues);

        if (allowAnyCharacterDialogues)
            result.AddRange(__instance.AgnosticDialogues);

        __result = result;

        // Skip original method.
        return false;
    }
}