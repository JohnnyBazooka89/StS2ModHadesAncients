using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Ancients;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
        List<AncientOption> options =
            HecateAncient.GetAllHecateAncientOptions()
                .Where(option => Owner.Relics.All(ownedRelic => ownedRelic.GetType() != option.GetType()))
                .ToList();

        Owner.PlayerRng.Rewards.Shuffle(options);

        if (options.Count > 0)
        {
            await RelicCmd.Obtain(options[0].ModelForOption, Owner);
        }

        if (options.Count > 1)
        {
            await RelicCmd.Obtain(options[1].ModelForOption, Owner);
        }
    }
}