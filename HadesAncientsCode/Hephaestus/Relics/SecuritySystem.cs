using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class SecuritySystem() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    private const string TurnsKey = "Turns";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnsKey, 3M)
    ];

    public override decimal ModifyBlockMultiplicative(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (target != Owner.Creature || props.HasFlag(ValueProp.Unpowered) || cardSource == null)
        {
            return 1;
        }

        return Owner.Creature.CombatState!.RoundNumber <= DynamicVars[TurnsKey].BaseValue ? 2M : 1M;
    }
}