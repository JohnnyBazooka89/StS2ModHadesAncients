using BaseLib.Common.Rewards;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Zeus.Relics;

[Pool(typeof(EventRelicPool))]
public class ShockingLoss() : HadesAncientsRelic(HadesAncient.Zeus)
{
    private const string CombatsKey = "Combats";
    private int _combatsSeen;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override int DisplayAmount => CombatsSeen % DynamicVars[CombatsKey].IntValue;

    public override bool ShowCounter => true;

    [SavedProperty]
    private int CombatsSeen
    {
        get => _combatsSeen;
        set
        {
            AssertMutable();
            _combatsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(CombatsKey, 3M)
    ];

    public override Task AfterCombatEnd(CombatRoom room)
    {
        ++CombatsSeen;
        if (CombatsSeen % DynamicVars[CombatsKey].IntValue == 0)
        {
            Flash();
            room.AddExtraReward(Owner, new CardRemovalReward(Owner));
        }

        return Task.CompletedTask;
    }
}