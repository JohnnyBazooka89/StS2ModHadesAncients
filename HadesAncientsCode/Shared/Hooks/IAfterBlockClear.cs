using MegaCrit.Sts2.Core.Entities.Creatures;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterBlockClear
{
    public Task AfterBlockClear(Creature creature, int blockBeforeClearing);
}