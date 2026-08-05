using HadesAncients.HadesAncientsCode.Shared.Hooks;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace HadesAncients.HadesAncientsCode.Shared.Patches.Hooks;

[HarmonyPatch(
    typeof(CardCmd),
    nameof(CardCmd.Upgrade), typeof(IEnumerable<CardModel>), typeof(CardPreviewStyle))]
public class AfterCardBecameUpgradedOrEnchanted_Upgrade_Patch
{
    public static void Prefix(
        ref IEnumerable<CardModel> cards,
        out CardState[] __state)
    {
        List<CardModel> cardList = cards.ToList();
        cards = cardList;

        __state = cardList
            .Select(card => new CardState(
                card,
                CardUtils.IsUpgradedOrEnchanted(card)))
            .ToArray();
    }

    public static void Postfix(CardState[] __state)
    {
        CardModel[] transitionedCards = __state
            .Where(state =>
                !state.WasUpgradedOrEnchanted &&
                CardUtils.IsUpgradedOrEnchanted(state.Card))
            .Select(state => state.Card)
            .ToArray();

        if (transitionedCards.Length == 0)
        {
            return;
        }

        _ = TaskHelper.RunSafely(
            DispatchTransitionsSequentially(transitionedCards));
    }

    private static async Task DispatchTransitionsSequentially(
        IEnumerable<CardModel> cards)
    {
        foreach (CardModel card in cards)
        {
            await HadesAncientsHooks
                .AfterCardBecameUpgradedOrEnchanted(
                    card.Owner?.RunState,
                    card.Owner?.Creature.CombatState,
                    card);
        }
    }

    public readonly record struct CardState(
        CardModel Card,
        bool WasUpgradedOrEnchanted);
}