using BaseLib.Abstracts;
using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public abstract class HadesAncientsRelic(HadesAncient hadesAncient) : CustomRelicModel
{
    public override string PackedIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath(hadesAncient);
    public override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicOutlineImagePath(hadesAncient);
    public override string BigIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath(hadesAncient);
}