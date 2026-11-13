using BaseLib.Cards.Variables;
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

public class ChillPower() : HadesAncientsPower(HadesAncient.Demeter), IModifyDamageAdditiveCompatibility,
    IAddDumbVariablesToPowerDescription
{
    private const decimal DamageReductionPerStackDefaultValue = 1M;
    private const string DamageReductionPerStackKey = "DamageReductionPerStack";
    private const string DamageReductionKey = "DamageReduction";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(DamageReductionPerStackKey, DamageReductionPerStackDefaultValue),
        new(DamageReductionKey + "Base", 0M),
        new(DamageReductionKey + "Extra", DamageReductionPerStackDefaultValue),
        new CustomCalculatedDamageVar(DamageReductionKey, ValueProp.Unpowered)
            .WithMultiplier(static (power, _) => power.Amount)
    ];

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add(
            HadesAncientsMainFile.ModId + nameof(ChillPower) + DamageReductionPerStackKey,
            DamageReductionPerStackDefaultValue
        );
    }

    public decimal ModifyDamageAdditiveCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner)
            return 0M;

        return -((CustomCalculatedDamageVar)DynamicVars[DamageReductionKey]).CalculateCustom(Owner);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await PowerCmd.Remove(this);
    }
}