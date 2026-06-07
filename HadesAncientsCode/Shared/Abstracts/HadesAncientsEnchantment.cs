using BaseLib.Abstracts;
using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public abstract class HadesAncientsEnchantment(HadesAncient hadesAncient) : CustomEnchantmentModel
{
    protected override string CustomIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath(hadesAncient);
}