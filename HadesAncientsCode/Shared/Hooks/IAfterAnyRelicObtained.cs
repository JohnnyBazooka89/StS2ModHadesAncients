using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterAnyRelicObtained
{
    Task AfterAnyRelicObtained(Player player, RelicModel relic);
}