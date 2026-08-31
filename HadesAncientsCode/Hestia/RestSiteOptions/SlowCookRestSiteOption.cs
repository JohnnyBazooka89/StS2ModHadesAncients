using HadesAncients.HadesAncientsCode.Hestia.Relics;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Hestia.RestSiteOptions;

public class SlowCookRestSiteOption(Player owner, SlowCookParams slowCookParams)
    : HadesAncientsRestSiteOption(HadesAncient.Hestia, owner)
{
    public override string OptionId => "SLOW_COOK";

    public override LocString Description
    {
        get
        {
            LocString description = new LocString("static_hover_tips", "HADESANCIENTS-SLOW_COOK.description");
            description.Add(SlowCookParams.CardsToRemoveKey, slowCookParams.CardsToRemove);
            description.Add(SlowCookParams.StrengthToGainKey, slowCookParams.StrengthToGain);
            return description;
        }
    }

    public override async Task<bool> OnSelect()
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, slowCookParams.CardsToRemove)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            };
        List<CardModel> source = (await CardSelectCmd.FromDeckForRemoval(Owner, prefs)).ToList();
        if (source.Count == 0)
            return false;
        foreach (CardModel card in source)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        SlowCooker? slowCooker = Owner.GetRelic<SlowCooker>();
        slowCooker?.IncreaseCombatStartStrength();

        return true;
    }
}

public sealed record SlowCookParams(
    int CardsToRemove,
    int StrengthToGain)
{
    public const string CardsToRemoveKey = "CardsToRemove";
    public const string StrengthToGainKey = "StrengthToGain";
}