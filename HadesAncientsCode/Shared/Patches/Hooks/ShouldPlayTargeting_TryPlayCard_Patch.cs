using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(typeof(NCardPlay), "TryPlayCard")]
public static class ShouldPlayTargeting_TryPlayCard_Patch
{
    public static bool Prefix(NCardPlay __instance, Creature? target)
    {
        CardModel? card = __instance.Holder?.CardModel;
        if (card == null)
            return true;

        Player? owner = card.Owner;
        if (owner == null)
            return true;

        Creature? effectiveTarget = IsSingleCreatureTarget(card)
            ? target
            : null;

        IRunState? runState = card.RunState;
        ICombatState? combatState =
            card.CombatState ?? owner.Creature.CombatState;

        if (HadesAncientsHooks.ShouldPlayTargeting(
                runState,
                combatState,
                card,
                effectiveTarget,
                AutoPlayType.None,
                out AbstractModel? preventer))
        {
            return true;
        }

        ShowBlockedThoughtBubble(owner, preventer);

        __instance.CancelPlayCard();

        // Skip original NCardPlay.TryPlayCard.
        return false;
    }

    private static bool IsSingleCreatureTarget(CardModel card)
    {
        return card.TargetType is TargetType.AnyEnemy or TargetType.AnyAlly;
    }

    private static void ShowBlockedThoughtBubble(Player owner, AbstractModel? preventer)
    {
        if (preventer == null)
            return;

        LocString? playerDialogueLine =
            UnplayableReason.BlockedByHook.GetPlayerDialogueLine(preventer);

        if (playerDialogueLine == null)
            return;

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
            NThoughtBubbleVfx.Create(
                playerDialogueLine.GetFormattedText(),
                owner.Creature,
                1.0
            )
        );
    }
}