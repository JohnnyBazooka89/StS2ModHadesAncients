using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterCardBecameUpgradedOrEnchanted
{
    Task AfterCardBecameUpgradedOrEnchanted(CardModel card);
}