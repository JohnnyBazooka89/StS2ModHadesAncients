using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

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

    public static int GetDamageForForecast(Creature? dealer, Creature target, Decimal damage, ValueProp valueProp)
    {
        return (int)Hook.ModifyDamage(target.CombatState!.RunState, target.CombatState, target, dealer,
            damage, valueProp, null, null, ModifyDamageHookType.All, CardPreviewMode.None,
            out IEnumerable<AbstractModel> _);
    }
}