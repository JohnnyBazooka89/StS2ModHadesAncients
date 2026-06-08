using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class DefensivePosture() : HadesAncientsRelic(HadesAncient.Athena), IModifyHpLostAfterOstyLateFinal
{
    private bool _usedThisCombat;

    private bool UsedThisCombat
    {
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
        }
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public decimal ModifyHpLostAfterOstyLateFinal(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Status != RelicStatus.Active ||
            amount <= 0 ||
            target != Owner.Creature ||
            (!Owner.Creature.CombatState?.Enemies.Contains(dealer) ?? true))
        {
            return amount;
        }

        Flash();
        UsedThisCombat = true;
        return 0;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        if (UsedThisCombat)
        {
            Status = RelicStatus.Disabled;
        }

        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        UsedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}