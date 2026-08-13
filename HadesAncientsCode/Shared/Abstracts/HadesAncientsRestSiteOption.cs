using BaseLib.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public abstract class HadesAncientsRestSiteOption(HadesAncient hadesAncient, Player owner) : CustomRestSiteOption(owner)
{
    public override string CustomIconPath => $"{OptionId.ToLowerInvariant()}.png".RestSiteOptionImagePath(hadesAncient);
}