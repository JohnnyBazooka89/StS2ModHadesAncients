using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Unlocks;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(
    typeof(ActModel),
    nameof(ActModel.GenerateRooms), typeof(Rng), typeof(UnlockState), typeof(bool))]
public static class HadesAncients_DisableBaseGameAncients_Patch
{
    private static readonly MethodInfo FilterAncientsMethod =
        AccessTools.Method(
            typeof(HadesAncients_DisableBaseGameAncients_Patch),
            nameof(FilterAncients));

    private static readonly MethodInfo EnumerableConcatMethod =
        AccessTools.Method(typeof(Enumerable), nameof(Enumerable.Concat))
            .MakeGenericMethod(typeof(AncientEventModel));

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;

            // After:
            // this.GetUnlockedAncients(unlockState)
            //     .Concat(this._sharedAncientSubset ?? new List<AncientEventModel>())
            //
            // inject:
            //     .FilterAncients()
            if (instruction.Calls(EnumerableConcatMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, FilterAncientsMethod);
            }
        }
    }

    private static IEnumerable<AncientEventModel> FilterAncients(IEnumerable<AncientEventModel> ancients)
    {
        if (!HadesAncientsModConfig.DisableBaseGameAncients)
            return ancients;

        return ancients
            .Where(ancient =>
                ancient is not Nonupeipe &&
                ancient is not Tanx &&
                ancient is not Vakuu &&
                ancient is not Orobas &&
                ancient is not Pael &&
                ancient is not Tezcatara &&
                ancient is not Darv)
            .ToArray();
    }
}