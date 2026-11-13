using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Demeter.Relics;

[Pool(typeof(EventRelicPool))]
public class SnowQueen() : HadesAncientsRelic(HadesAncient.Demeter)
{
    private const string MaxHpLossPercentKey = "MaxHpLossPercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(MaxHpLossPercentKey, 50M),
        new PowerVar<BufferPower>(2),
        new PowerVar<PlatingPower>(10)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    public override async Task AfterObtained()
    {
        int maxHpLoss = (int)(Owner.Creature.MaxHp * DynamicVars[MaxHpLossPercentKey].BaseValue / 100M);
        await CreatureCmdUtils.LoseMaxHpSafely(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            maxHpLoss,
            false);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        Flash();
        await PowerCmd.Apply<BufferPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars[nameof(BufferPower)].BaseValue,
            Owner.Creature,
            null);
        await PowerCmd.Apply<PlatingPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars[nameof(PlatingPower)].BaseValue,
            Owner.Creature,
            null);
    }
}
