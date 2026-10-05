using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Artemis.Cards;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class KillingStroke() : HadesAncientsRelic(HadesAncient.Artemis)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromCardWithCardHoverTips<CleanKill>()
    ];

    public override async Task AfterObtained()
    {
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(Owner.RunState.CreateCard<CleanKill>(Owner), PileType.Deck), 2f);
    }
}