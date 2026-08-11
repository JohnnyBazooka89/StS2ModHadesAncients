using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class StabbingRush() : HadesAncientsRelic(HadesAncient.Ares)
{
    private const string CardsToExhaustKey = "CardsToExhaust";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new(CardsToExhaustKey, 2)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != Owner ? count : count + DynamicVars.Cards.BaseValue;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
            return;
        foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, player,
                     new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt,
                         DynamicVars[CardsToExhaustKey].IntValue), null, this))
            await CardCmd.Exhaust(choiceContext, card);
    }
}