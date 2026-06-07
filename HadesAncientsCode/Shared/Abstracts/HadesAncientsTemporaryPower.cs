using BaseLib.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Abstracts;

public abstract class HadesAncientsTemporaryPower<TModel, TPower>(HadesAncient hadesAncient)
    : CustomTemporaryPowerModelWrapper<TModel, TPower>
    where TModel : AbstractModel
    where TPower : PowerModel
{
    public override string CustomPackedIconPath => HadesAncientsPowerIconPaths.PackedIconPath(Id.Entry, hadesAncient);

    public override string CustomBigIconPath => HadesAncientsPowerIconPaths.BigIconPath(Id.Entry, hadesAncient);
}