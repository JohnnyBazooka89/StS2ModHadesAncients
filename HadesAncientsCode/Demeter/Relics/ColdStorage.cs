using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Demeter.Enchantments;
using HadesAncients.HadesAncientsCode.Demeter.Enums;
using HadesAncients.HadesAncientsCode.Demeter.RestSiteOptions;
using HadesAncients.HadesAncientsCode.Demeter.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class ColdStorage() : HadesAncientsRelic(HadesAncient.Demeter)
{
    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(DemeterStaticHoverTips.Preserve, CanonicalVars.ToArray()),
        ..HoverTipFactory.FromEnchantment<Icebound>()
    ];

    public override Task AfterObtained()
    {
        return PreserveUtils.SelectAndEnchant(Owner, DynamicVars.Cards.IntValue, cancelable: false);
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner || !PileType.Deck.GetPile(player).Cards.Any(ModelDb.Enchantment<Icebound>().CanEnchant))
        {
            return false;
        }

        options.Add(new PreserveRestSiteOption(player, new ColdStorageParams(DynamicVars.Cards.IntValue)));
        return true;
    }
}