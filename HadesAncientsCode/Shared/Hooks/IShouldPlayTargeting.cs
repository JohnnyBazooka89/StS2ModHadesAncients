using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IShouldPlayTargeting
{
    bool ShouldPlayTargeting(CardModel card, Creature? cardTarget);
}