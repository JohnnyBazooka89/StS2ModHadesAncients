using BaseLib.Patches.Localization;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Dionysus.Powers;

public class FestiveFogPower() : HadesAncientsPower(HadesAncient.Dionysus), IModifyDamageMultiplicativeCompatibility,
    IAddDumbVariablesToPowerDescription
{
    private const decimal DamageTakenIncreaseDefaultValue = 50;
    private const decimal DamageDealtDecreaseDefaultValue = 25;

    private const string DamageTakenIncreaseKey = "DamageTakenIncrease";
    private const string DamageDealtDecreaseKey = "DamageDealtDecrease";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(DamageTakenIncreaseKey, DamageTakenIncreaseDefaultValue),
        new(DamageDealtDecreaseKey, DamageDealtDecreaseDefaultValue)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(this)
    ];

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add(
            HadesAncientsMainFile.ModId + nameof(FestiveFogPower) + DamageTakenIncreaseKey,
            DamageTakenIncreaseDefaultValue
        );

        description.Add(
            HadesAncientsMainFile.ModId + nameof(FestiveFogPower) + DamageDealtDecreaseKey,
            DamageDealtDecreaseDefaultValue
        );
    }

    public Decimal ModifyDamageMultiplicativeCompatibility(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || dealer == target)
            return 1M;

        Decimal multiplier = 1M;

        if (dealer == Owner)
        {
            multiplier *= 1M - DynamicVars[DamageDealtDecreaseKey].BaseValue / 100M;
        }

        if (target == Owner)
        {
            multiplier *= 1M + DynamicVars[DamageTakenIncreaseKey].BaseValue / 100M;
        }

        return multiplier;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        if (Owner.IsAlive)
        {
            await PowerCmd.Decrement(this);
        }
    }
}