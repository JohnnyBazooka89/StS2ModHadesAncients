using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Ancients;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Judgement() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string RelicsToObtainKey = "RelicsToObtain";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(RelicsToObtainKey, 2M)
    ];

    public int GetArcanaRelicNumber()
    {
        return 25;
    }

    public override async Task AfterActEntered()
    {
        Flash();
        List<RelicModel> optionRelics =
            HecateAncient.GetAllHecateAncientOptions()
                .Select(option => option.ModelForOption)
                .Where(optionRelic => Owner.Relics.All(ownedRelic => ownedRelic.GetType() != optionRelic.GetType()))
                .ToList();

        Owner.PlayerRng.Rewards.Shuffle(optionRelics);

        if (optionRelics.Count > 0)
        {
            await RelicCmd.Obtain(optionRelics[0], Owner);
        }

        if (optionRelics.Count > 1)
        {
            await RelicCmd.Obtain(optionRelics[1], Owner);
        }
    }
}