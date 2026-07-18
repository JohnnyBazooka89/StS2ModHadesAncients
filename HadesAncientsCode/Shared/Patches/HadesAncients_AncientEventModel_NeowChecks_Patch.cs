using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HadesAncients.HadesAncientsCode.Chaos.Ancients;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch]
public static class HadesAncients_AncientEventModel_NeowChecks_Patch
{
    private static readonly MethodInfo IsNeowLikeMethod =
        AccessTools.Method(
            typeof(HadesAncients_AncientEventModel_NeowChecks_Patch),
            nameof(IsNeowLike))
        ?? throw new MissingMethodException(
            nameof(HadesAncients_AncientEventModel_NeowChecks_Patch),
            nameof(IsNeowLike));

    /// <summary>
    ///     BeforeEventStarted is async, so its actual body is inside the
    ///     compiler-generated IAsyncStateMachine.MoveNext method.
    /// </summary>
    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        MethodInfo beforeEventStarted =
            AccessTools.Method(
                typeof(AncientEventModel),
                "BeforeEventStarted",
                [typeof(bool)])
            ?? throw new MissingMethodException(
                typeof(AncientEventModel).FullName,
                "BeforeEventStarted");

        AsyncStateMachineAttribute stateMachineAttribute =
            beforeEventStarted.GetCustomAttribute<AsyncStateMachineAttribute>()
            ?? throw new InvalidOperationException(
                "BeforeEventStarted does not have an AsyncStateMachineAttribute.");

        return AccessTools.Method(
                   stateMachineAttribute.StateMachineType,
                   nameof(IAsyncStateMachine.MoveNext))
               ?? throw new MissingMethodException(
                   stateMachineAttribute.StateMachineType.FullName,
                   nameof(IAsyncStateMachine.MoveNext));
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        int replacements = 0;

        foreach (CodeInstruction instruction in instructions)
        {
            // Original:
            // ancientEventModel is Neow
            //
            // IL:
            // isinst Neow
            //
            // Replacement:
            // IsNeowLike(ancientEventModel)
            if (instruction.opcode == OpCodes.Isinst &&
                instruction.operand is Type checkedType &&
                checkedType == typeof(Neow))
            {
                // Mutating the existing instruction preserves its
                // labels and exception-block metadata.
                instruction.opcode = OpCodes.Call;
                instruction.operand = IsNeowLikeMethod;

                replacements++;
            }

            yield return instruction;
        }

        // The supplied game method currently contains exactly two checks:
        //
        // 1. SetCurrentHpInternal(0)
        // 2. TopBar.Hp.LerpAtNeow()
        if (replacements != 2)
        {
            throw new InvalidOperationException(
                $"Expected to replace 2 Neow checks in " +
                $"{nameof(AncientEventModel)}.BeforeEventStarted, " +
                $"but replaced {replacements}. The game method may have changed.");
        }
    }

    /// <summary>
    ///     Behaves like an extended `is Neow` instruction.
    ///     Returning the original object or null preserves the stack behavior
    ///     of `isinst Neow`, which also returns an object reference or null.
    /// </summary>
    private static object? IsNeowLike(object? ancient)
    {
        return ancient is Neow or ChaosAncient
            ? ancient
            : null;
    }
}