using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace HadesAncients.HadesAncientsCode.Athena.Patches;

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.IsOverOrEnding), MethodType.Getter)]
public static class MentalBlock_FixGainingBlock_Patch
{
    public static readonly AsyncLocal<bool> ForceFalse = new();

    public static void Postfix(ref bool __result)
    {
        if (ForceFalse.Value)
            __result = false;
    }
}