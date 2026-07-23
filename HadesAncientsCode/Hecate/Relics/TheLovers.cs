using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Powers;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
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

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheLovers() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<InfatuatedPower>(1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InfatuatedPower>()];

    public int GetArcanaRelicNumber()
    {
        return 15;
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState!.TurnNumber > 1 ||
            (Owner.RunState.CurrentRoom?.RoomType != RoomType.Elite &&
             Owner.RunState.CurrentRoom?.RoomType != RoomType.Boss))
            return;
        Flash();
        await PowerCmd.Apply<InfatuatedPower>(choiceContext, combatState.HittableEnemies,
            DynamicVars[nameof(InfatuatedPower)].BaseValue, Owner.Creature, null);
    }
}