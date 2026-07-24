using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheBoatman() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private bool _wasUsed;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    [SavedProperty]
    private bool WasUsed
    {
        get => _wasUsed;
        set
        {
            AssertMutable();
            _wasUsed = value;
            if (!IsUsedUp)
                return;
            Status = RelicStatus.Disabled;
        }
    }

    public override bool IsUsedUp => _wasUsed;

    public int GetArcanaRelicNumber()
    {
        return 17;
    }

    public override Task AfterItemPurchased(Player player, MerchantEntry itemPurchased, int goldSpent)
    {
        if (itemPurchased is not MerchantRelicEntry)
        {
            return Task.CompletedTask;
        }

        WasUsed = true;
        return Task.CompletedTask;
    }

    public override Decimal ModifyMerchantPrice(
        Player player,
        MerchantEntry entry,
        Decimal originalPrice)
    {
        return WasUsed || player != Owner || entry is not MerchantRelicEntry ? originalPrice : 0;
    }
}