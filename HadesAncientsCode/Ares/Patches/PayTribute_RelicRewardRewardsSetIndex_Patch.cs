using System.Reflection;
using BaseLib.Common.Rewards;
using HadesAncients.HadesAncientsCode.Ares.SpireFields;
using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;

namespace HadesAncients.HadesAncientsCode.Ares.Patches;

[HarmonyPatch]
public static class PayTribute_RelicRewardRewardsSetIndex_Patch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.PropertyGetter(
            typeof(RelicReward),
            nameof(Reward.RewardsSetIndex)
        );

        yield return AccessTools.PropertyGetter(
            typeof(CardUpgradeReward),
            nameof(Reward.RewardsSetIndex)
        );

        yield return AccessTools.PropertyGetter(
            typeof(CardRemovalReward),
            nameof(Reward.RewardsSetIndex)
        );

        yield return AccessTools.PropertyGetter(
            typeof(PotionReward),
            nameof(Reward.RewardsSetIndex)
        );
    }

    [HarmonyPostfix]
    private static void Postfix(Reward __instance, ref int __result)
    {
        int? index = AresSpireFields.PayTributeRewardsSetIndex.Get(__instance);

        if (index.HasValue)
            __result = index.Value;
    }
}