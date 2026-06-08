using BaseLib.Cards.Variables;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Vars;

public class OutsideCombatCalculatedVar(string name) : CustomCalculatedVar(name)
{
    private static readonly AccessTools.FieldRef<CustomCalculatedVar, Func<PowerModel, Creature, Decimal>>
        PowerCalcRef =
            AccessTools.FieldRefAccess<CustomCalculatedVar, Func<PowerModel, Creature, Decimal>>("_powerCalc");

    private static readonly AccessTools.FieldRef<CustomCalculatedVar, Func<RelicModel, Creature, Decimal>>
        RelicCalcRef =
            AccessTools.FieldRefAccess<CustomCalculatedVar, Func<RelicModel, Creature, Decimal>>("_relicCalc");


    public override decimal CalculateCustom(Creature? target)
    {
        switch (_owner)
        {
            case CardModel:
                return Calculate(target);
            case PowerModel power:
                var mult = PowerCalcRef(this)?.Invoke(power, target) ??
                           throw new InvalidOperationException(
                               $"CustomCalculatedVar {Name} does not have multiplier calc defined for powers in {_owner.Id}");
                return GetBaseVar().BaseValue + GetExtraVar().BaseValue * mult;
            case RelicModel relic:
                mult = RelicCalcRef(this)?.Invoke(relic, target) ?? throw new InvalidOperationException(
                    $"CustomCalculatedVar {Name} does not have multiplier calc defined for relics in {_owner.Id}");
                return GetBaseVar().BaseValue + GetExtraVar().BaseValue * mult;
            default:
                return BaseValue;
        }
    }
}