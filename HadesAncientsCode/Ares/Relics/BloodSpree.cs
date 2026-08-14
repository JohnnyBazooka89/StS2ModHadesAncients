using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Dionysus.Relics;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class BloodSpree() : HadesAncientsRelic(HadesAncient.Ares)
{
    private const string MaxHpLossKey = "MaxHpLoss";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GoldVar(400),
        new(MaxHpLossKey, 4M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromRelic<CharonsObol>()
    ];

    public override async Task AfterObtained()
    {
        await RelicCmd.Obtain(ModelDb.Relic<CharonsObol>().ToMutable(), Owner);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not MerchantRoom)
            return;

        Flash();
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
        await CreatureCmdUtils.LoseMaxHpSafely(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars[MaxHpLossKey].IntValue, false);
    }
}