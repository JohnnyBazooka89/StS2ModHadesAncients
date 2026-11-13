using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Demeter.Cards;

[Pool(typeof(EventCardPool))]
public class WeedOut()
    : HadesAncientsCard(HadesAncient.Demeter, 1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    private const decimal DamageUpgradeValue = 4M;
    private const decimal BlockUpgradeValue = 2M;

    public override bool GainsBlock => true;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12M, ValueProp.Move),
        new BlockVar(6M, ValueProp.Move),
        new CardsVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmdCompatibility
            .FromCard(DamageCmd.Attack(DynamicVars.Damage.BaseValue), this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (PileType.Hand.GetPile(Owner).Cards.All(card => card == this))
            return;

        List<CardModel> cardsToExhaust = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, DynamicVars.Cards.IntValue),
            null,
            this)).ToList();

        if (!cardsToExhaust.Any())
            return;

        foreach (CardModel cardToExhaust in cardsToExhaust)
        {
            await CardCmdCompatibility.Exhaust(choiceContext, cardToExhaust);
            if (cardToExhaust.Pile?.Type != PileType.Exhaust)
                return;

            await DamageCmdCompatibility
                .FromCard(DamageCmd.Attack(DynamicVars.Damage.BaseValue), this, cardPlay)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(DamageUpgradeValue);
        DynamicVars.Block.UpgradeValueBy(BlockUpgradeValue);
    }
}