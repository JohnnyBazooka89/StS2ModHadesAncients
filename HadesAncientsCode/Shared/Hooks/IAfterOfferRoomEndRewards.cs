using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterOfferRoomEndRewards
{
    Task AfterOfferRoomEndRewards(CombatRoom room);
}