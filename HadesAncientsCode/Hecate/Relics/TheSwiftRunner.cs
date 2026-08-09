using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class TheSwiftRunner() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public int GetArcanaRelicNumber()
    {
        return 11;
    }
}