using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HadesAncients.HadesAncientsCode.Dionysus.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Dionysus.Patches;

[HarmonyPatch]
public static class TipsyShot_TriggerPotionsTwice_Patch
{
    private static readonly MethodInfo ReplacementMethod =
        AccessTools.Method(
            typeof(TipsyShot_TriggerPotionsTwice_Patch),
            nameof(CallOnUseMaybeTwice));

    static MethodBase TargetMethod()
    {
        MethodInfo wrapper = AccessTools.Method(
            typeof(PotionModel),
            nameof(PotionModel.OnUseWrapper));

        var asyncAttr = wrapper.GetCustomAttribute<AsyncStateMachineAttribute>();

        if (asyncAttr == null)
            throw new Exception("Tipsy Shot patch failed: OnUseWrapper is not async.");

        MethodInfo moveNext = AccessTools.Method(
            asyncAttr.StateMachineType,
            "MoveNext");

        if (moveNext == null)
            throw new Exception("Tipsy Shot patch failed: could not find MoveNext.");

        return moveNext;
    }

    static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        bool patched = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (IsOnUseCall(instruction))
            {
                patched = true;
                yield return new CodeInstruction(OpCodes.Call, ReplacementMethod);
                continue;
            }

            yield return instruction;
        }

        if (!patched)
        {
            HadesAncientsMainFile.Logger.Warn("Tipsy Shot patch warning: could not find PotionModel.OnUse call.");
        }
    }

    private static bool IsOnUseCall(CodeInstruction instruction)
    {
        if (instruction.opcode != OpCodes.Call &&
            instruction.opcode != OpCodes.Callvirt)
            return false;

        if (instruction.operand is not MethodInfo method)
            return false;

        return method.Name == "OnUse"
               && typeof(Task).IsAssignableFrom(method.ReturnType);
    }

    public static async Task CallOnUseMaybeTwice(
        PotionModel potion,
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        await potion.OnUse(choiceContext, target);

        Player? owner = potion.Owner;
        if (owner?.GetRelic<TipsyShot>() == null)
            return;

        await potion.OnUse(choiceContext, target);
    }
}