using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheUnseen() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string EnergyToGainKey = "EnergyToGain";

    private int _energySpent;
    private bool _isActivating;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? EnergySpent : DynamicVars.Energy.IntValue;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(9),
        new EnergyVar(EnergyToGainKey, 1)
    ];

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    private int EnergySpent
    {
        get => _energySpent;
        set
        {
            AssertMutable();
            _energySpent = value;
            InvokeDisplayAmountChanged();
        }
    }

    public int GetArcanaRelicNumber()
    {
        return 9;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.IsAutoPlay)
        {
            return;
        }

        EnergySpent += cardPlay.Resources.EnergySpent;
        while (EnergySpent >= DynamicVars.Energy.IntValue)
        {
            _ = TaskHelper.RunSafely(DoActivateVisuals());

            await PlayerCmd.GainEnergy(DynamicVars[EnergyToGainKey].BaseValue, Owner);

            EnergySpent -= DynamicVars.Energy.IntValue;
        }

        Status = EnergySpent == DynamicVars.Energy.IntValue - 1
            ? RelicStatus.Active
            : RelicStatus.Normal;
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}