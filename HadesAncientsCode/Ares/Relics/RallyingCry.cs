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
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class RallyingCry() : HadesAncientsRelic(HadesAncient.Ares)
{
    private const string GloryToGainKey = "GloryToGain";
    private int _glory;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => Glory;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(GloryToGainKey, 1M),
        new PowerVar<StrengthPower>(2M),
        new PowerVar<ThornsPower>(2M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<ThornsPower>()
    ];

    [SavedProperty]
    private int Glory
    {
        get => _glory;
        set
        {
            AssertMutable();
            _glory = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner.Creature.IsDead)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            Glory * DynamicVars.Strength.IntValue, Owner.Creature, null);

        await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            Glory * DynamicVars[nameof(ThornsPower)].IntValue, Owner.Creature, null);
    }

    public override Task AfterCombatVictory(CombatRoom combatRoom)
    {
        if (Owner.Creature.IsDead || combatRoom.RoomType != RoomType.Elite)
            return Task.CompletedTask;

        Glory++;
        Flash();

        return Task.CompletedTask;
    }
}