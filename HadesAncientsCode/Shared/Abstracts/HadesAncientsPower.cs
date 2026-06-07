using BaseLib.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public abstract class HadesAncientsPower(HadesAncient hadesAncient) : CustomPowerModel
{
    public override string CustomPackedIconPath => HadesAncientsPowerIconPaths.PackedIconPath(Id.Entry, hadesAncient);

    public override string CustomBigIconPath => HadesAncientsPowerIconPaths.BigIconPath(Id.Entry, hadesAncient);
}