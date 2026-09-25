using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Athena.Potions;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class RenewedFaith() : HadesAncientsRelic(HadesAncient.Athena)
{
    private const string PotionsToGainKey = "PotionsToGain";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(PotionsToGainKey, 1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPotion<DivineReprieve>(),
        ..ModelDb.Potion<DivineReprieve>().HoverTips
    ];

    public override async Task AfterObtained()
    {
        int originalSlotCount = Owner.MaxPotionCount;
        await PlayerCmd.GainMaxPotionCount(DynamicVars[PotionsToGainKey].IntValue, Owner);
        PotionModel potionModel = ModelDb.Get<DivineReprieve>();
        await PotionCmd.TryToProcure(potionModel.ToMutable(), Owner, originalSlotCount);
    }
}