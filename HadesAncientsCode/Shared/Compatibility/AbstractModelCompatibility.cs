using System.Collections.Concurrent;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public static class AbstractModelCompatibility
{
    private const string ModifyDamageCapMethodName = "ModifyDamageCap";

    private static readonly ConcurrentDictionary<Type, MethodInfo> ModifyDamageCapMethods = new();

    public static decimal ModifyDamageCap(
        AbstractModel model,
        Creature? target,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay = null
    )
    {
        MethodInfo method = ModifyDamageCapMethods.GetOrAdd(
            model.GetType(),
            FindModifyDamageCapMethod
        );

        object?[] args = method.GetParameters().Length == 5
            ? [target, props, dealer, cardSource, cardPlay]
            : [target, props, dealer, cardSource];

        object? result = method.Invoke(model, args);

        if (result is decimal cap)
            return cap;

        throw new InvalidOperationException(
            $"Unexpected return type from {model.GetType().FullName}.ModifyDamageCap: {result?.GetType().FullName ?? "null"}"
        );
    }

    private static MethodInfo FindModifyDamageCapMethod(Type modelType)
    {
        MethodInfo? method = modelType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.Name == ModifyDamageCapMethodName)
            .FirstOrDefault(IsSupportedModifyDamageCapSignature);

        if (method == null)
        {
            throw new MissingMethodException(
                modelType.FullName,
                ModifyDamageCapMethodName
            );
        }

        return method;
    }

    private static bool IsSupportedModifyDamageCapSignature(MethodInfo method)
    {
        ParameterInfo[] p = method.GetParameters();

        if (p.Length is not (4 or 5))
            return false;

        if (p[0].ParameterType != typeof(Creature))
            return false;

        if (p[1].ParameterType != typeof(ValueProp))
            return false;

        if (p[2].ParameterType != typeof(Creature))
            return false;

        if (p[3].ParameterType != typeof(CardModel))
            return false;

        if (p.Length == 5 && p[4].ParameterType != typeof(CardPlay))
            return false;

        return method.ReturnType == typeof(decimal);
    }
}