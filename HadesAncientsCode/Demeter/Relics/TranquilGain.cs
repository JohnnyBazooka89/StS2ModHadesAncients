using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class TranquilGain() : HadesAncientsRelic(HadesAncient.Demeter)
{
    private const string EnergyThresholdKey = "EnergyThreshold";
    private bool _activated;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(EnergyThresholdKey, 2M),
        new EnergyVar(1),
        new CardsVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    private bool Activated
    {
        get => _activated;
        set
        {
            AssertMutable();
            _activated = value;
        }
    }

    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature) || Activated ||
            Owner.PlayerCombatState!.Energy < DynamicVars[EnergyThresholdKey].BaseValue)
            return Task.CompletedTask;

        Activated = true;
        Status = RelicStatus.Active;
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || !Activated)
            return;

        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), DynamicVars.Cards.BaseValue, Owner);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Activated = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}
