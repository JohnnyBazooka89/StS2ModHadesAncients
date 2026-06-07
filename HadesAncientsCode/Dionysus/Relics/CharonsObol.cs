using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class CharonsObol() : HadesAncientsRelic(HadesAncient.Dionysus), IAfterRoomTypeRolled
{
    private bool _wasUsed;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsUsedUp => _wasUsed;

    [SavedProperty]
    public bool WasUsed
    {
        get => _wasUsed;
        set
        {
            AssertMutable();
            _wasUsed = value;
            if (!IsUsedUp)
                return;
            Status = RelicStatus.Disabled;
        }
    }

    public void AfterRoomTypeRolled(IRunState runState, RoomType roomType)
    {
        if (roomType != RoomType.Shop || WasUsed)
        {
            return;
        }

        Flash();
        WasUsed = true;
    }

    public override IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(
        IReadOnlySet<RoomType> roomTypes)
    {
        if (WasUsed)
        {
            return roomTypes;
        }

        HashSet<RoomType> newRoomTypes = new HashSet<RoomType>();
        newRoomTypes.Add(RoomType.Shop);
        return newRoomTypes;
    }
}