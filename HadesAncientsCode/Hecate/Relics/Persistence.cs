using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Persistence()
    : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string MoreDamagePercentKey = "MoreDamagePercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(6M),
        new EnergyVar(2)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public int GetArcanaRelicNumber()
    {
        return 7;
    }

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState!.TurnNumber > 1 ||
            (Owner.RunState.CurrentRoom?.RoomType != RoomType.Elite &&
             Owner.RunState.CurrentRoom?.RoomType != RoomType.Boss)
           )
        {
            return;
        }

        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}