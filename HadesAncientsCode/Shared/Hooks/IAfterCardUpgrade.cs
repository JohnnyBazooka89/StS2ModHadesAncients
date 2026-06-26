using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterCardUpgrade
{
    Task AfterCardUpgrade(CardModel card);
}