using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Powers;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheFuries() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private bool UsedThisCombat { get; set; }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TormentPower>(1)
    ];

    public int GetArcanaRelicNumber()
    {
        return 6;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if ((dealer != Owner.Creature && dealer != Owner.Osty) || !props.IsPoweredAttack() || UsedThisCombat)
        {
            return;
        }

        UsedThisCombat = true;
        Status = RelicStatus.Normal;
        Flash();
        await PowerCmd.Apply<TormentPower>(choiceContext, target, 2M, Owner.Creature, null);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
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