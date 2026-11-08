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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Powers;

public class IceStruckPower() : HadesAncientsPower(HadesAncient.Demeter), IModifyDamageMultiplicativeCompatibility,
    IAddDumbVariablesToPowerDescription
{
    private const decimal TurnsDefaultValue = 1M;
    private const decimal LessDamagePercentDefaultValue = 50M;

    private const string TurnsKey = "Turns";
    private const string LessDamagePercentKey = "LessDamagePercent";

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(LessDamagePercentKey, LessDamagePercentDefaultValue)
    ];

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add(
            HadesAncientsMainFile.ModId + nameof(IceStruckPower) + LessDamagePercentKey,
            LessDamagePercentDefaultValue
        );
        description.Add(
            HadesAncientsMainFile.ModId + nameof(IceStruckPower) + TurnsKey,
            TurnsDefaultValue
        );
    }

    public decimal ModifyDamageMultiplicativeCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack())
            return 1M;

        return 1M - DynamicVars[LessDamagePercentKey].BaseValue / 100M;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;

        await PowerCmd.TickDownDuration(this);
    }
}