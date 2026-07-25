using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyHpLost))]
public static class ModifyHpLostAfterOstyFinal_ModifyHpLost_Patch
{
    public static void Postfix(
        IRunState runState,
        ICombatState? combatState,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        HpLossHookPhase phases,
        ref IEnumerable<AbstractModel> modifiers,
        ref decimal __result)
    {
        if (!phases.HasFlag(HpLossHookPhase.AfterOsty))
            return;

        __result = HadesAncientsHooks.ModifyHpLostAfterOstyFinal(
            runState,
            combatState,
            target,
            __result,
            props,
            dealer,
            cardSource,
            ref modifiers
        );
    }
}