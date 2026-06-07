using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterRoomTypeRolled
{
    void AfterRoomTypeRolled(IRunState runState, RoomType roomType);
}