using HadesAncients.HadesAncientsCode.Hecate.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Patches;

[HarmonyPatch(
    typeof(MapTravel),
    nameof(MapTravel.GetTravelablePointsFrom)
)]
public class TheSwiftRunner_MapTravel_GetTravelablePointsFrom_Patch
{
    [HarmonyPostfix]
    private static void Postfix(
        IRunState runState,
        MapPoint currentPoint,
        ref IEnumerable<MapPoint> __result)
    {
        bool relicIsActive = runState.Players.Any(player => player.Relics.Any(relic => relic is TheSwiftRunner));

        if (!relicIsActive)
            return;

        MapPoint[] nextRow = runState.Map
            .GetPointsInRow(currentPoint.coord.row + 1)
            .ToArray();

        // FROM a Shop: every point in the next row is reachable.
        if (currentPoint.PointType == MapPointType.Shop)
        {
            __result = nextRow;
            return;
        }

        // TO a Shop: add every Shop in the next row to the normally
        // reachable children.
        __result = __result
            .Concat(
                nextRow.Where(point => point.PointType == MapPointType.Shop
                )
            )
            .Distinct()
            .ToArray();
    }
}