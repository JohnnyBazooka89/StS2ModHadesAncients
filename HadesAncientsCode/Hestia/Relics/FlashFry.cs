using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class FlashFry() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string FavorStartCombatKey = "FavorStartCombat";
    private const string FavorLossEachTurnKey = "FavorLossEachTurn";

    private int _favor;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => Favor;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(FavorStartCombatKey, 5),
        new(FavorLossEachTurnKey, 1),
        new GoldVar(30),
    ];

    private int Favor
    {
        get => _favor;
        set
        {
            AssertMutable();
            _favor = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || combatState.RoundNumber != 1)
            return Task.CompletedTask;
        Favor = 5;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature) || Favor <= 0)
            return Task.CompletedTask;

        Favor--;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Creature.IsDead || Favor <= 0)
            return Task.CompletedTask;

        Flash();
        room.AddExtraReward(Owner, new GoldReward(Favor * DynamicVars.Gold.IntValue, Owner));
        if (room.RoomType == RoomType.Elite)
        {
            room.AddExtraReward(Owner, new RelicReward(Owner));
        }

        Favor = 0;
        return Task.CompletedTask;
    }
}