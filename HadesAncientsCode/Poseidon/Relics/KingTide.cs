using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Poseidon.Relics;

[Pool(typeof(EventRelicPool))]
public class KingTide() : HadesAncientsRelic(HadesAncient.Poseidon)
{
    private const string NormalCombatStrengthKey = "NormalCombatStrength";
    private const string EliteCombatStrengthKey = "EliteCombatStrength";
    private const string BossCombatStrengthKey = "BossCombatStrength";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(NormalCombatStrengthKey, 1M),
        new(EliteCombatStrengthKey, 3M),
        new(BossCombatStrengthKey, 5M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner.Creature.IsDead)
        {
            return;
        }

        int strengthToGain;
        if (room.RoomType == RoomType.Boss)
        {
            strengthToGain = DynamicVars[BossCombatStrengthKey].IntValue;
        }
        else if (room.RoomType == RoomType.Elite)
        {
            strengthToGain = DynamicVars[EliteCombatStrengthKey].IntValue;
        }
        else
        {
            strengthToGain = DynamicVars[NormalCombatStrengthKey].IntValue;
        }

        Flash();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            strengthToGain, Owner.Creature, null);
    }
}