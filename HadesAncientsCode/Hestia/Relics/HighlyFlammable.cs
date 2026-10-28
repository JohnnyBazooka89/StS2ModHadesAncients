using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class HighlyFlammable() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string MoreStacksKey = "MoreStacks";
    private PowerModel? _ignoredInternalPower;

    private List<PendingTemporary>? _pending;

    private List<PendingTemporary> Pending
    {
        get
        {
            AssertMutable();
            return _pending ??= [];
        }
    }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MoreStacksKey, 2)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool IsEligible(
        PowerModel power,
        decimal amount,
        Creature? giver,
        Creature? target)
    {
        return giver == Owner.Creature
               && target != null
               && Owner.Creature.CombatState?.Enemies.Contains(target) == true
               && power.GetTypeForAmount(amount) == PowerType.Debuff;
    }

    public override Task BeforePowerAmountChanged(
        PowerModel power,
        decimal amount,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        AssertMutable();
        _ignoredInternalPower = null;

        if (!IsEligible(power, amount, applier, target))
            return Task.CompletedTask;

        // Check whether this is an internal application
        // from a previously doubled temporary debuff.
        int index = Pending.FindIndex(p =>
            p.InternalType == power.GetType()
            && ReferenceEquals(p.Target, target)
            && ReferenceEquals(p.Applier, applier)
            && ReferenceEquals(p.CardSource, cardSource));

        if (index >= 0)
        {
            // Skip doubling this internal application.
            _ignoredInternalPower = power;
            Pending.RemoveAt(index);
        }
        else if (power is ITemporaryPower temporary)
        {
            // Remember which power this temporary debuff will apply.
            Pending.Add(new PendingTemporary(
                power,
                temporary.InternallyAppliedPower.GetType(),
                target,
                applier!,
                cardSource,
                power.Amount
            ));
        }

        return Task.CompletedTask;
    }

    public override decimal ModifyPowerAmountGivenAdditive(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        if (!IsEligible(power, amount, giver, target))
            return 0;

        // Already enhanced through its temporary power.
        if (ReferenceEquals(power, _ignoredInternalPower))
            return 0;

        return 2 * Math.Sign(amount);
    }

    public override Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        // Remove pending entries if the temporary power
        // wasn't actually changed (e.g. blocked).
        Pending.RemoveAll(p =>
            ReferenceEquals(p.Source, power)
            && p.AmountBefore == power.Amount);

        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext chaoiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (ReferenceEquals(power, _ignoredInternalPower))
        {
            AssertMutable();
            _ignoredInternalPower = null;
        }

        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // Remove unmatched entries left by this card.
        Pending.RemoveAll(p =>
            ReferenceEquals(p.CardSource, cardPlay.Card));

        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        Pending.Clear();
        _ignoredInternalPower = null;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Pending.Clear();
        _ignoredInternalPower = null;
        return Task.CompletedTask;
    }

    private sealed record PendingTemporary(
        PowerModel Source,
        Type InternalType,
        Creature Target,
        Creature Applier,
        CardModel? CardSource,
        int AmountBefore
    );
}