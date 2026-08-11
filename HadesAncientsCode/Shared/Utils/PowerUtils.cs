using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Utils;

public class PowerUtils
{
    public static int GetUniqueDebuffsCount(Creature? creature)
    {
        return creature?.Powers
            .Where(power => power.TypeForCurrentAmount == PowerType.Debuff && power is not ITemporaryPower)
            .Select(power => power.Id)
            .Distinct()
            .Count() ?? 0;
    }
}