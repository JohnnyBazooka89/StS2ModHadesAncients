using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheArtificer() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string TimesKey = "Times";
    private int _charges;

    [SavedProperty]
    private int Charges
    {
        get => _charges;
        set
        {
            AssertMutable();
            _charges = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override int DisplayAmount => Charges;

    public override bool ShowCounter => true;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TimesKey, 2M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 18;
    }

    public override Task AfterPotionDiscarded(PotionModel potion)
    {
        if (Charges <= 0 || potion.Owner != Owner || Owner.RunState.CurrentRoom is not CombatRoom combatRoom ||
            !CombatManager.Instance.IsInProgress)
        {
            return Task.CompletedTask;
        }

        Charges--;
        Flash();
        combatRoom.AddExtraReward(Owner, new RelicReward(Owner));
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        Charges = DynamicVars[TimesKey].IntValue;
        return Task.CompletedTask;
    }
}