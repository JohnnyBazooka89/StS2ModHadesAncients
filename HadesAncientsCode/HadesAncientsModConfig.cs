using BaseLib.Config;

namespace HadesAncients.HadesAncientsCode;

public class HadesAncientsModConfig : SimpleModConfig
{
    [ConfigSection("AncientsAct1")] //
    public static bool DisableHecate { get; set; } = false;
    [ConfigSection("AncientsAct2")] //
    public static bool DisableAthena { get; set; } = false;
    public static bool DisablePoseidon { get; set; } = false;
    public static bool DisableZeus { get; set; } = false;
    [ConfigSection("AncientsAct3")] // 
    public static bool DisableAphrodite { get; set; } = false;
    public static bool DisableDionysus { get; set; } = false;
    public static bool DisableHephaestus { get; set; } = false;

    [ConfigSection("BaseGameAncients")] //
    public static bool DisableNeow { get; set; } = false;
    public static bool DisableBaseGameAncients { get; set; } = false;

    [ConfigSection("Poseidon")] //
    public static bool PoseidonDisableSeaStarSoundEffects { get; set; } = false;
}