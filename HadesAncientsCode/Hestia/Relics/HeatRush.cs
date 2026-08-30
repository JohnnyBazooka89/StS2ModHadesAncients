using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
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

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class HeatRush() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string CardsToExhaustKey = "CardsToExhaust";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new(CardsToExhaustKey, 1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
            return;
        foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, player,
                     new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt,
                         DynamicVars[CardsToExhaustKey].IntValue), null, this))
            await CardCmdCompatibility.Exhaust(choiceContext, card);
    }
}