using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterLoadRun
{
    Task AfterLoadRun(SerializableRoom? room);
}