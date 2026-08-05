using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(
    typeof(CardCmd),
    nameof(CardCmd.Enchant), typeof(EnchantmentModel), typeof(CardModel), typeof(decimal))]
public static class AfterCardBecameUpgradedOrEnchanted_Enchant_Patch
{
    public static void Prefix(
        CardModel card,
        out bool __state)
    {
        __state = CardUtils.IsUpgradedOrEnchanted(card);
    }

    public static void Postfix(
        CardModel card,
        bool __state)
    {
        bool wasUpgradedOrEnchanted = __state;
        bool isUpgradedOrEnchanted =
            CardUtils.IsUpgradedOrEnchanted(card);

        if (wasUpgradedOrEnchanted || !isUpgradedOrEnchanted)
        {
            return;
        }

        _ = TaskHelper.RunSafely(
            HadesAncientsHooks
                .AfterCardBecameUpgradedOrEnchanted(
                    card.Owner?.RunState,
                    card.Owner?.Creature.CombatState,
                    card));
    }
}