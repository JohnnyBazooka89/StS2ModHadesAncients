using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Hecate.Ancients;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch]
public static class HadesAncients_AddAct1Ancients_Patch
{
    private static readonly IReadOnlyList<ModelId> Act1AncientIds =
    [
        ModelDb.GetId<HecateAncient>()
    ];

    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo abstractMethod = AccessTools.DeclaredMethod(
            typeof(ActModel),
            nameof(ActModel.GetUnlockedAncients),
            [typeof(UnlockState)]);

        return AccessTools.AllTypes()
            .Where(type =>
                type != typeof(ActModel) &&
                typeof(ActModel).IsAssignableFrom(type))
            .Select(type => AccessTools.Method(
                type,
                nameof(ActModel.GetUnlockedAncients),
                [typeof(UnlockState)]))
            .Where(method =>
                method is not null &&
                !method.IsAbstract &&
                method.GetBaseDefinition() == abstractMethod)
            .Distinct();
    }

    [HarmonyPostfix]
    private static IEnumerable<AncientEventModel> Postfix(
        IEnumerable<AncientEventModel> ancients,
        ActModel __instance)
    {
        if (__instance.ActNumber() != 1)
            return ancients;

        List<AncientEventModel> result = ancients.ToList();

        foreach (ModelId ancientId in Act1AncientIds)
        {
            CustomAncientModel ancient =
                ModelDb.GetById<CustomAncientModel>(ancientId);

            if (ancient.IsValidForAct(__instance))
                result.Add(ancient);
        }

        return result;
    }
}