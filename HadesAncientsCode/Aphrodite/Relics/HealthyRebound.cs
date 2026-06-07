using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Relics;

[Pool(typeof(EventRelicPool))]
public class HealthyRebound() : HadesAncientsRelic(HadesAncient.Aphrodite)
{
    private const string NormalCombatHealKey = "NormalCombatHeal";
    private const string EliteCombatHealKey = "EliteCombatHeal";
    private const string BossCombatHealKey = "BossCombatHeal";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(NormalCombatHealKey, 5M),
        new(EliteCombatHealKey, 15M),
        new(BossCombatHealKey, 25M)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner.Creature.IsDead)
        {
            return;
        }

        int amountToHeal;
        if (room.RoomType == RoomType.Boss)
        {
            amountToHeal = DynamicVars[BossCombatHealKey].IntValue;
        }
        else if (room.RoomType == RoomType.Elite)
        {
            amountToHeal = DynamicVars[EliteCombatHealKey].IntValue;
        }
        else
        {
            amountToHeal = DynamicVars[NormalCombatHealKey].IntValue;
        }

        Flash();
        await CreatureCmd.Heal(Owner.Creature, amountToHeal);
    }
}