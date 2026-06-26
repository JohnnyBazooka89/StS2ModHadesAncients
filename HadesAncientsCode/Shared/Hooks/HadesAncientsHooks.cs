using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public class HadesAncientsHooks
{
    private static async Task DispatchAsync<T>(IRunState? runState, ICombatState? combatState, Func<T, Task> action)
        where T : class
    {
        foreach (var model in runState?.IterateHookListeners(combatState).OfType<T>() ?? [])
        {
            await action(model);
        }
    }

    private static void Dispatch<T>(IRunState? runState, ICombatState? combatState, Action<T> action)
        where T : class
    {
        foreach (var model in runState?.IterateHookListeners(combatState).OfType<T>() ?? [])
        {
            action(model);
        }
    }

    public static Task AfterAnyRelicObtained(IRunState? rs, ICombatState? cs, Player player, RelicModel relic)
    {
        return DispatchAsync<IAfterAnyRelicObtained>(rs, cs, m => m.AfterAnyRelicObtained(player, relic));
    }

    public static async Task AfterArtifactPowerModifiedPowerAmountReceived(
        IRunState? runState,
        ICombatState? combatState,
        ArtifactPower artifactPower,
        PowerModel blockedPower
    )
    {
        await DispatchAsync<IAfterArtifactPowerModifiedPowerAmountReceived>(
            runState,
            combatState,
            model => model.AfterArtifactPowerModifiedPowerAmountReceived(
                artifactPower,
                blockedPower
            )
        );
    }

    public static async Task AfterCardUpgrade(IRunState? runState,
        ICombatState? combatState,
        CardModel cardModel)
    {
        await DispatchAsync<IAfterCardUpgrade>(
            runState,
            combatState,
            model => model.AfterCardUpgrade(
                cardModel
            )
        );
    }

    public static async Task AfterLoadRun(RunState runState, SerializableRoom? preFinishedRoom)
    {
        await DispatchAsync<IAfterLoadRun>(
            runState,
            null,
            model => model.AfterLoadRun(
                preFinishedRoom
            )
        );
    }

    public static void AfterRoomTypeRolled(IRunState runState, RoomType roomType)
    {
        Dispatch<IAfterRoomTypeRolled>(runState, null, m => m.AfterRoomTypeRolled(runState, roomType));
    }

    public static Decimal ModifyDamageToFinalValue(IRunState? runState, ICombatState? combatState, Creature? target,
        Decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPreviewMode previewMode,
        ref IEnumerable<AbstractModel> modifiers)
    {
        Boolean changed = false;
        foreach (IModifyDamageToFinalValue model in runState?.IterateHookListeners(combatState)
                     .OfType<IModifyDamageToFinalValue>() ?? [])
        {
            decimal num = model.ModifyDamageToFinalValue(target, amount, props, dealer, cardSource, previewMode);
            if (num != amount)
            {
                if (model is AbstractModel abstractModel)
                {
                    modifiers = modifiers.Append(abstractModel);
                }

                amount = num;
                changed = true;
            }
        }

        if (changed)
        {
            Decimal cappedDamage = Decimal.MaxValue;
            foreach (AbstractModel iterateHookListener in runState!.IterateHookListeners(combatState))
            {
                Decimal capToCompare = iterateHookListener.ModifyDamageCap(target, props, dealer, cardSource);
                if (capToCompare < cappedDamage)
                {
                    cappedDamage = capToCompare;
                    if (amount > cappedDamage)
                    {
                        amount = cappedDamage;
                    }
                }
            }
        }

        return amount;
    }

    public static decimal ModifyHpLostAfterOstyLateFinal(
        IRunState? runState,
        ICombatState? combatState,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        ref IEnumerable<AbstractModel> modifiers)
    {
        foreach (IModifyHpLostAfterOstyLateFinal model in runState?.IterateHookListeners(combatState)
                     .OfType<IModifyHpLostAfterOstyLateFinal>() ?? [])
        {
            decimal oldAmount = amount;

            amount = model.ModifyHpLostAfterOstyLateFinal(
                target,
                amount,
                props,
                dealer,
                cardSource
            );

            if (decimal.Truncate(oldAmount) != decimal.Truncate(amount))
            {
                if (model is AbstractModel abstractModel)
                {
                    modifiers = modifiers.Append(abstractModel);
                }
            }
        }

        return amount;
    }

    public static decimal ModifyHpLostBeforeOstyAfterLate(
        IRunState? runState,
        ICombatState? combatState,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        ref IEnumerable<AbstractModel> modifiers)
    {
        foreach (IModifyHpLostBeforeOstyAfterLate model in runState?.IterateHookListeners(combatState)
                     .OfType<IModifyHpLostBeforeOstyAfterLate>() ?? [])
        {
            decimal oldAmount = amount;

            amount = model.ModifyHpLostBeforeOstyAfterLate(
                target,
                amount,
                props,
                dealer,
                cardSource
            );

            if (decimal.Truncate(oldAmount) != decimal.Truncate(amount))
            {
                if (model is AbstractModel abstractModel)
                {
                    modifiers = modifiers.Append(abstractModel);
                }
            }
        }

        return amount;
    }

    public static bool ShouldPlayTargeting(
        IRunState? runState,
        ICombatState? combatState,
        CardModel card,
        Creature? cardTarget,
        AutoPlayType autoPlayType,
        out AbstractModel? preventer)
    {
        preventer = null;

        foreach (AbstractModel model in runState?.IterateHookListeners(combatState) ?? [])
        {
            if (model is not IShouldPlayTargeting shouldPlay || shouldPlay.ShouldPlayTargeting(card, cardTarget))
                continue;
            preventer = model;
            return false;
        }

        return true;
    }
}