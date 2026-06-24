using BaseLib.Cards.Variables;
using BaseLib.Hooks;
using Godot;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Powers;

public class BlastPower() : HadesAncientsPower(HadesAncient.Hephaestus)
{
    private const string HalfAmountKey = "HalfAmount";

    private static readonly Color Color = new(65 / 255f, 43 / 255f, 21 / 255f);

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HalfAmountKey + "Base", 0M),
        new(HalfAmountKey + "Extra", 1M),
        new CustomCalculatedVar(HalfAmountKey).WithMultiplier(static (power, target) =>
            Math.Ceiling(power.Amount / 2M))
    ];

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unpowered,
            Applier!);

        if (Owner.IsAlive)
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this,
                -((CustomCalculatedVar)DynamicVars[HalfAmountKey]).CalculateCustom(null), null, null);
        }
    }

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        return
        [
            new HealthBarForecastSegment(
                Math.Max(0, Amount - context.Creature.Block), Color,
                HealthBarForecastDirection.FromRight)
        ];
    }
}