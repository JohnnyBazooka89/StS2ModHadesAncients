using HadesAncients.HadesAncientsCode.Poseidon.SpireFields;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Poseidon.Patches;

[HarmonyPatch(typeof(CardReward))]
[HarmonyPatch(
    MethodType.Constructor,
    typeof(IEnumerable<CardModel>),
    typeof(CardCreationSource),
    typeof(Player),
    typeof(CardCreationOptions),
    typeof(PlayerChoiceSynchronizer)
)]
public static class SeaStar_CardRewardManualCardsCtor_Patch
{
    private static void Postfix(CardReward __instance, Player player)
    {
        var cardsToOffer = __instance._cards
            .Select(result => player.RunState.CloneCard(result.Card))
            .ToList();

        PoseidonSpireFields.SeaStarOriginalCards.Set(__instance, cardsToOffer);
    }
}