using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Odds;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(UnknownMapPointOdds), nameof(UnknownMapPointOdds.Roll))]
public static class AfterRoomTypeRolled_UnknownMapPointOddsRoll_Patch
{
    public static void Postfix(IRunState runState, RoomType __result)
    {
        HadesAncientsHooks.AfterRoomTypeRolled(runState, __result);
    }
}