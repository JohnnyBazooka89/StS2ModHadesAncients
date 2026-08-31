using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class ControlledBurn() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string BurnAlternativeKey = "BURN";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        if (Owner != player)
            return false;
        alternatives.Add(new CardRewardAlternative(BurnAlternativeKey, OnSacrifice,
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private Task OnSacrifice()
    {
        Flash();
        List<CardModel> list = PileType.Deck.GetPile(Owner).Cards
            .Where(c => c.IsUpgradable).ToList()
            .StableShuffle(Owner.RunState.Rng.Niche).Take(1).ToList();
        if (list.Count == 0)
            return Task.CompletedTask;
        foreach (CardModel card in list)
            CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
}