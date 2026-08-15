using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(NRelicInventoryHolder), "RefreshAmount")]
public static class HadesAncients_ShowCustomRelicStringLabel_Patch
{
    [HarmonyPostfix]
    public static void Postfix(NRelicInventoryHolder __instance,
        MegaLabel ____amountLabel)
    {
        if (__instance.Relic.Model is not ICustomRelicStringLabel customRelicStringLabel)
            return;

        if (customRelicStringLabel.ShowCustomStringDisplayLabel && RunManager.Instance.IsInProgress)
        {
            ____amountLabel.Visible = true;
            ____amountLabel.SetTextAutoSize(customRelicStringLabel.CustomStringDisplayLabel);
        }
        else
        {
            ____amountLabel.Visible = false;
        }
    }
}