using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Chaos.Ancients;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Unlocks;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(
    typeof(ActModel),
    nameof(ActModel.GenerateRooms), typeof(Rng), typeof(UnlockState), typeof(bool))]
public static class HadesAncients_AddAct1Ancients_Patch
{
    private static readonly MethodInfo AddAncientsMethod =
        AccessTools.Method(
            typeof(HadesAncients_AddAct1Ancients_Patch),
            nameof(AddAncients),
            [
                typeof(IEnumerable<AncientEventModel>),
                typeof(ActModel)
            ]);

    private static readonly MethodInfo EnumerableConcatMethod =
        AccessTools.Method(typeof(Enumerable), nameof(Enumerable.Concat))
            .MakeGenericMethod(typeof(AncientEventModel));
    private static readonly IReadOnlyList<ModelId> Act1AncientsIds = [ModelDb.GetId<ChaosAncient>()];

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;

            // Stack after Concat:
            // [IEnumerable<AncientEventModel>]
            if (instruction.Calls(EnumerableConcatMethod))
            {
                // GenerateRooms is an instance method, so argument 0 is `this`.
                //
                // Stack becomes:
                // [IEnumerable<AncientEventModel>, ActModel]
                yield return new CodeInstruction(OpCodes.Ldarg_0);

                // Calls AddAncients(ancients, actModel).
                yield return new CodeInstruction(
                    OpCodes.Call,
                    AddAncientsMethod);
            }
        }
    }

    private static IEnumerable<AncientEventModel> AddAncients(IEnumerable<AncientEventModel> ancients,
        ActModel actModel)
    {
        var ancientEventModels = ancients.ToList();
        if (actModel.ActNumber() != 1)
        {
            return ancientEventModels;
        }

        foreach (ModelId act1AncientId in Act1AncientsIds)
        {
            CustomAncientModel act1Ancient = ModelDb.GetById<CustomAncientModel>(act1AncientId);
            if (act1Ancient.IsValidForAct(actModel))
            {
                ancientEventModels.Add(act1Ancient);
            }
        }

        return ancientEventModels;
    }
}