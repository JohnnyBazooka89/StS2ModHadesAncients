using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Dionysus.Enchantments;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class DrunkenDash() : HadesAncientsRelic(HadesAncient.Dionysus)
{
    private const string IntoxicateKey = "Intoxicate";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(5),
        new(IntoxicateKey, 4M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromEnchantment<Intoxicate>(DynamicVars[IntoxicateKey].IntValue)
    ];

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, DynamicVars.Cards.IntValue);
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Intoxicate>(),
                     DynamicVars[IntoxicateKey].IntValue,
                     prefs))
        {
            CardCmd.Enchant<Intoxicate>(card, DynamicVars[IntoxicateKey].BaseValue);
            NCardEnchantVfx child = NCardEnchantVfx.Create(card);
            if (child != null)
            {
                NRun instance = NRun.Instance;
                if (instance != null)
                    instance.GlobalUi.CardPreviewContainer.AddChildSafely(child);
            }
        }
    }
}