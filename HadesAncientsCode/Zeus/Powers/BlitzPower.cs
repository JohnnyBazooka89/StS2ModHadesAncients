using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Zeus.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Zeus.Powers;

public class BlitzPower() : HadesAncientsPower(HadesAncient.Zeus), IHasSecondAmount
{
    private const string UnblockedDamageLeftKey = "UnblockedDamageLeft";
    private const string BlitzDamageIncreaseKey = "BlitzDamageIncrease";
    private const string BlitzDamageKey = "BlitzDamage";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => DynamicVars[UnblockedDamageLeftKey].IntValue;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(UnblockedDamageLeftKey, 15M),
        new(BlitzDamageIncreaseKey, 4M),
        new(BlitzDamageKey + "Base", 8M),
        new(BlitzDamageKey + "Extra", 1M),
        new CustomCalculatedDamageVar(BlitzDamageKey, ValueProp.Unpowered).WithMultiplier(static (power, target) =>
                power.Owner.GetPowerAmount<HeavenStruckPower>())
    ];

    public string GetSecondAmount()
    {
        return $"{(int)((CustomCalculatedDamageVar)DynamicVars[BlitzDamageKey]).CalculateCustom(Owner)}";
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack())
        {
            return;
        }

        DynamicVars[UnblockedDamageLeftKey].BaseValue -=
            Math.Min(DynamicVars[UnblockedDamageLeftKey].BaseValue, result.UnblockedDamage);
        InvokeDisplayAmountChanged();
        if (DynamicVars[UnblockedDamageLeftKey].IntValue <= 0)
        {
            await ZeusUtils.DealLightningDamage(choiceContext, dealer, Owner,
                ((CustomCalculatedDamageVar)DynamicVars[BlitzDamageKey]).CalculateCustom(Owner));
            await PowerCmd.Apply<HeavenStruckPower>(choiceContext, Owner, DynamicVars[BlitzDamageIncreaseKey].IntValue,
                dealer, null);
            foreach (BlitzPower blitzPower in Owner.Powers.OfType<BlitzPower>())
            {
                blitzPower.InvokeSecondAmountChanged();
            }

            await PowerCmd.Remove(this);
        }
    }
}