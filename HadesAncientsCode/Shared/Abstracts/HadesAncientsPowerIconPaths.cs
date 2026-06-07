using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public static class HadesAncientsPowerIconPaths
{
    public static string PackedIconPath(string entry, HadesAncient hadesAncient) =>
        $"{entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath(hadesAncient);

    public static string BigIconPath(string entry, HadesAncient hadesAncient) =>
        $"{entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath(hadesAncient);
}