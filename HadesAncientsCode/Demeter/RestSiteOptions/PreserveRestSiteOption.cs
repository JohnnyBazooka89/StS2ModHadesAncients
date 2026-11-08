using HadesAncients.HadesAncientsCode.Demeter.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;

namespace HadesAncients.HadesAncientsCode.Demeter.RestSiteOptions;

public class PreserveRestSiteOption(Player owner, int cardsToPreserve)
    : HadesAncientsRestSiteOption(HadesAncient.Demeter, owner)
{
    public override string OptionId => "PRESERVE";

    public override LocString Description
    {
        get
        {
            LocString description = new("static_hover_tips", "HADESANCIENTS-PRESERVE.description");
            description.Add("Cards", cardsToPreserve);
            return description;
        }
    }

    public override Task<bool> OnSelect()
    {
        return PreserveUtils.SelectAndEnchant(Owner, cardsToPreserve, cancelable: true);
    }
}
