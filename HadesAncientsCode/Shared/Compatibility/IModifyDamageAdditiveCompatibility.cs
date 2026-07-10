using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public interface IModifyDamageAdditiveCompatibility
{
    decimal ModifyDamageAdditiveCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    );
}

[HarmonyPatch]
public static class ModifyDamageAdditiveCompatibilityPatch
{
    private const string MethodName = "ModifyDamageAdditive";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(AbstractModel)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.Name == MethodName)
            .Where(m =>
            {
                ParameterInfo[] parameters = m.GetParameters();

                // Old version:
                // ModifyDamageAdditive(target, amount, props, dealer, cardSource)

                // New version:
                // ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay)

                return parameters.Length is 5 or 6;
            });
    }

    private static void Postfix(
        AbstractModel __instance,
        ref decimal __result,
        object?[] __args
    )
    {
        if (__instance is not IModifyDamageAdditiveCompatibility compatibility)
            return;

        Creature? target = (Creature?)__args[0];
        decimal amount = (decimal)__args[1]!;
        ValueProp props = (ValueProp)__args[2]!;
        Creature? dealer = (Creature?)__args[3];
        CardModel? cardSource = (CardModel?)__args[4];

        CardPlay? cardPlay = __args.Length >= 6
            ? (CardPlay?)__args[5]
            : null;

        __result += compatibility.ModifyDamageAdditiveCompatibility(
            target,
            amount,
            props,
            dealer,
            cardSource,
            cardPlay
        );
    }
}