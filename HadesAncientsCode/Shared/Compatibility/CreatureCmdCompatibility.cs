using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public static class CreatureCmdCompatibility
{
    private static readonly MethodInfo DamageMethod = FindDamageMethod();

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay = null
    )
    {
        object?[] args = DamageMethod.GetParameters().Length == 7
            ? [choiceContext, target, amount, props, dealer, cardSource, cardPlay]
            : [choiceContext, target, amount, props, dealer, cardSource];

        object? result = DamageMethod.Invoke(null, args);

        if (result is Task<IEnumerable<DamageResult>> task)
            return task;

        throw new InvalidOperationException(
            $"Unexpected return type from CreatureCmd.Damage: {result?.GetType().FullName ?? "null"}"
        );
    }

    private static MethodInfo FindDamageMethod()
    {
        MethodInfo? method = typeof(CreatureCmd)
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.Name == nameof(CreatureCmd.Damage))
            .FirstOrDefault(IsSupportedDamageSignature);

        if (method == null)
        {
            throw new MissingMethodException(
                "Could not find a supported CreatureCmd.Damage overload."
            );
        }

        return method;
    }

    private static bool IsSupportedDamageSignature(MethodInfo method)
    {
        ParameterInfo[] p = method.GetParameters();

        if (p.Length is not (6 or 7))
            return false;

        if (p[0].ParameterType != typeof(PlayerChoiceContext))
            return false;

        if (p[1].ParameterType != typeof(Creature))
            return false;

        if (p[2].ParameterType != typeof(decimal))
            return false;

        if (p[3].ParameterType != typeof(ValueProp))
            return false;

        if (p[4].ParameterType != typeof(Creature))
            return false;

        if (p[5].ParameterType != typeof(CardModel))
            return false;

        if (p.Length == 7 && p[6].ParameterType != typeof(CardPlay))
            return false;

        return method.ReturnType == typeof(Task<IEnumerable<DamageResult>>);
    }
}