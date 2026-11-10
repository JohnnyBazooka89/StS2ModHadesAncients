using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyDamageInternal))]
public static class ModifyDamageToValue_ModifyDamageInternal_Patch
{
    static void Prefix(
        IRunState runState,
        ICombatState? combatState,
        Creature? target,
        Creature? dealer,
        ref decimal damage,
        ValueProp props,
        CardModel? cardSource,
        out IEnumerable<AbstractModel> __state)
    {
        __state = [];
        damage = HadesAncientsHooks.ModifyDamageToValue(
            runState,
            combatState,
            target,
            damage,
            props,
            dealer,
            cardSource,
            CardPreviewMode.None,
            ref __state
        );
    }

    static void Postfix(ref List<AbstractModel> modifiers, IEnumerable<AbstractModel> __state)
    {
        modifiers.AddRange(__state);
    }
}