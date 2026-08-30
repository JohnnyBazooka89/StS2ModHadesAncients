using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class CardioGain() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string HitsNeededKey = "HitsNeeded";

    private int _hitsDone;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => HitsDone;

    [SavedProperty]
    private int HitsDone
    {
        get => _hitsDone;
        set
        {
            AssertMutable();
            _hitsDone = value;
            UpdateDisplay();
        }
    }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HitsNeededKey, 5M),
        new EnergyVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
    ];

    private void UpdateDisplay()
    {
        Status = HitsDone == DynamicVars[HitsNeededKey].IntValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if ((dealer != Owner.Creature && dealer != Owner.Osty) || !props.IsPoweredAttack())
            return;

        ++HitsDone;
        HitsDone %= DynamicVars[HitsNeededKey].IntValue;
        if (HitsDone == 0)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }
}