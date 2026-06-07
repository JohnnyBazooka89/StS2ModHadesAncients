using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class RecklessAbandon() : HadesAncientsRelic(HadesAncient.Dionysus), IModifyDamageToFinalValue
{
    private const string FinalDamageValue1Key = "FinalDamageValue1";
    private const string FinalDamageValue2Key = "FinalDamageValue2";
    private const string FinalDamageValue3Key = "FinalDamageValue3";
    private const string FinalDamageProbability1Key = "FinalDamageProbability1";
    private const string FinalDamageProbability2Key = "FinalDamageProbability2";
    private const string FinalDamageProbability3Key = "FinalDamageProbability3";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(FinalDamageValue1Key, 5M),
        new(FinalDamageValue2Key, 11M),
        new(FinalDamageValue3Key, 55M),
        new(FinalDamageProbability1Key, 40M),
        new(FinalDamageProbability2Key, 50M),
        new(FinalDamageProbability3Key, 10M),
    ];

    public Decimal ModifyDamageToFinalValue(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPreviewMode previewMode)
    {
        if (!props.IsPoweredAttack() || cardSource == null || (dealer != Owner.Creature && dealer != Owner.Osty) ||
            previewMode != CardPreviewMode.None)
        {
            return amount;
        }

        var probability1 = DynamicVars[FinalDamageProbability1Key].BaseValue;
        var probability2 = DynamicVars[FinalDamageProbability2Key].BaseValue;

        var roll = Owner.RunState.Rng.Niche.NextInt(0, 100);

        if (roll < probability1)
        {
            return DynamicVars[FinalDamageValue1Key].BaseValue;
        }

        if (roll < probability1 + probability2)
        {
            return DynamicVars[FinalDamageValue2Key].BaseValue;
        }

        return DynamicVars[FinalDamageValue3Key].BaseValue;
    }

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }
}