using HadesAncients.HadesAncientsCode.Hecate.SpireFields;
using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;

namespace HadesAncients.HadesAncientsCode.Hecate.Patches;

class TheFates_Rewards_Patches
{
    [HarmonyPatch(typeof(CardReward), nameof(CardReward.CanReroll), MethodType.Getter)]
    internal static class CardRewardCanRerollPatch
    {
        [HarmonyPostfix]
        private static void Postfix(CardReward __instance, ref bool __result)
        {
            // Preserve Driftwood and any other native reroll source,
            // while additionally allowing The Furies reroll.
            __result |= HecateSpireFields.TheFuriesRerolls.Get(__instance) > 0;
        }
    }

    [HarmonyPatch(typeof(CardReward), nameof(CardReward.Reroll))]
    internal static class CardRewardRerollPatch
    {
        // Read the unpatched native value rather than calling CanReroll.
        // Calling the property would also include The Furies postfix.
        private static readonly AccessTools.FieldRef<CardReward, bool>
            NativeCanReroll = AccessTools.FieldRefAccess<CardReward, bool>(
                "<CanReroll>k__BackingField"
            );

        [HarmonyPrefix]
        private static void Prefix(CardReward __instance)
        {
            // Driftwood or another native effect currently owns this reroll.
            // Do not spend a Furies reroll.
            if (NativeCanReroll(__instance))
                return;

            int remaining =
                HecateSpireFields.TheFuriesRerolls.Get(__instance);

            if (remaining <= 0)
                return;

            HecateSpireFields.TheFuriesRerolls.Set(
                __instance,
                remaining - 1
            );
        }
    }
}