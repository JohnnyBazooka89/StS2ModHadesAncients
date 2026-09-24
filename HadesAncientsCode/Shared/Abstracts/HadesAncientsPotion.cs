using BaseLib.Abstracts;
using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public abstract class HadesAncientsPotion(HadesAncient hadesAncient) : CustomPotionModel
{
    public override string CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath(hadesAncient);

    public override string CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath(hadesAncient);
}