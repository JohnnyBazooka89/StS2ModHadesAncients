using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Vars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class PersonalLoan() : HadesAncientsRelic(HadesAncient.Dionysus)
{
    private const string TotalGoldKey = "TotalGold";
    private int _lentMoney;
    private bool _wasUsed;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsUsedUp => _wasUsed;

    [SavedProperty]
    public int LentMoney
    {
        get => _lentMoney;
        set
        {
            AssertMutable();
            _lentMoney = value;
        }
    }

    [SavedProperty]
    public bool WasUsed
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

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GoldVar(1250),
        new(TotalGoldKey + "Base", 0M),
        new(TotalGoldKey + "Extra", 1M),
        new OutsideCombatCalculatedVar(TotalGoldKey).WithMultiplier((relic, _) =>
        {
            int lentMoney = relic is PersonalLoan personalLoan ? personalLoan.LentMoney : 0;
            return lentMoney + relic.DynamicVars.Gold.IntValue;
        })
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromRelic<CharonsObol>()
    ];

    public override async Task AfterObtained()
    {
        LentMoney = Owner.Gold;
        await PlayerCmd.LoseGold(Owner.Gold, Owner);
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType != RoomType.Elite || WasUsed)
        {
            return Task.CompletedTask;
        }

        Flash();

        WasUsed = true;
        room.AddExtraReward(Owner,
            new GoldReward((int)((OutsideCombatCalculatedVar)DynamicVars[TotalGoldKey]).CalculateCustom(null), Owner));
        room.AddExtraReward(Owner, new RelicReward(ModelDb.Relic<CharonsObol>().ToMutable(), Owner));

        return Task.CompletedTask;
    }
}