using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace HadesAncients.HadesAncientsCode.Zeus.Enchantments;

public class Thundercall() : HadesAncientsEnchantment(HadesAncient.Zeus)
{
    private const string LightningToChannelKey = "LightningToChannel";

    public override bool HasExtraCardText => true;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(LightningToChannelKey, 1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromOrb<LightningOrb>()
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay?.Card != Card)
        {
            return;
        }

        for (int i = 0; i < DynamicVars[LightningToChannelKey].IntValue; i++)
        {
            await CreatureCmd.TriggerAnim(cardPlay.Card.Owner.Creature, "Cast",
                cardPlay.Card.Owner.Character.CastAnimDelay);
            await OrbCmd.Channel<LightningOrb>(choiceContext, cardPlay.Card.Owner);
        }
    }


    public override bool CanEnchant(CardModel c)
    {
        if (!base.CanEnchant(c))
            return false;
        return c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Defend);
    }
}