using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public static class ModifyDamageCompatibility
{
    private static readonly MethodInfo ModifyDamageMethod;
    private static readonly bool HasCardPlayParameter;

    static ModifyDamageCompatibility()
    {
        ModifyDamageMethod = typeof(Hook)
                                 .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                 .Where(m => m.Name == "ModifyDamage")
                                 .FirstOrDefault(IsCompatibleModifyDamageMethod)
                             ?? throw new MissingMethodException(
                                 typeof(Hook).FullName,
                                 "ModifyDamage");

        HasCardPlayParameter =
            ModifyDamageMethod.GetParameters().Length == 11;
    }

    public static decimal ModifyDamage(
        IRunState runState,
        ICombatState? combatState,
        Creature? target,
        Creature? dealer,
        decimal damage,
        ValueProp props,
        CardModel? cardSource,
        ModifyDamageHookType modifyDamageHookType,
        CardPreviewMode previewMode,
        out IEnumerable<AbstractModel> modifiers)
    {
        object?[] args;

        if (HasCardPlayParameter)
        {
            args =
            [
                runState,
                combatState,
                target,
                dealer,
                damage,
                props,
                cardSource,
                // CardPlay?
                null,
                modifyDamageHookType,
                previewMode,
                // out IEnumerable<AbstractModel>
                null
            ];
        }
        else
        {
            args =
            [
                runState,
                combatState,
                target,
                dealer,
                damage,
                props,
                cardSource,
                modifyDamageHookType,
                previewMode,
                // out IEnumerable<AbstractModel>
                null
            ];
        }

        var result = (decimal)ModifyDamageMethod.Invoke(null, args)!;

        modifiers = (IEnumerable<AbstractModel>)args[^1]!;

        return result;
    }

    private static bool IsCompatibleModifyDamageMethod(MethodInfo method)
    {
        if (method.ReturnType != typeof(decimal))
            return false;

        var parameters = method.GetParameters();

        if (parameters.Length is not (10 or 11))
            return false;

        if (parameters[0].ParameterType != typeof(IRunState)
            || parameters[1].ParameterType != typeof(ICombatState)
            || parameters[2].ParameterType != typeof(Creature)
            || parameters[3].ParameterType != typeof(Creature)
            || parameters[4].ParameterType != typeof(decimal)
            || parameters[5].ParameterType != typeof(ValueProp)
            || parameters[6].ParameterType != typeof(CardModel))
        {
            return false;
        }

        var offset = parameters.Length == 11 ? 1 : 0;

        return parameters[7 + offset].ParameterType == typeof(ModifyDamageHookType)
               && parameters[8 + offset].ParameterType == typeof(CardPreviewMode)
               && parameters[9 + offset].IsOut
               && parameters[9 + offset].ParameterType
               == typeof(IEnumerable<AbstractModel>).MakeByRefType();
    }
}