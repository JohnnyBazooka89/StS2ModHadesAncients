using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Ares.Enums;
using HadesAncients.HadesAncientsCode.Ares.RestSiteOptions;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class SanguinarySavor() : HadesAncientsRelic(HadesAncient.Ares)
{
    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(PayTributeParams.MaxHpLossKey, 5),
        new(PayTributeParams.RelicsToObtainKey, 1),
        new(PayTributeParams.CardsToUpgradeKey, 1),
        new(PayTributeParams.CardsToRemoveKey, 1),
        new(PayTributeParams.PotionsToProcureKey, 1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(AresStaticHoverTips.PayTribute, CanonicalVars.ToArray())
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
            return false;
        options.Add(new PayTributeRestSiteOption(player, new PayTributeParams(
            MaxHpLoss: DynamicVars[PayTributeParams.MaxHpLossKey].IntValue,
            RelicsToObtain: DynamicVars[PayTributeParams.RelicsToObtainKey].IntValue,
            CardsToUpgrade: DynamicVars[PayTributeParams.CardsToUpgradeKey].IntValue,
            CardsToRemove: DynamicVars[PayTributeParams.CardsToRemoveKey].IntValue,
            PotionsToProcure: DynamicVars[PayTributeParams.PotionsToProcureKey].IntValue
        )));
        return true;
    }
}