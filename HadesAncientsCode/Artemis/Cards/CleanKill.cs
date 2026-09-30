using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Artemis.Cards;

[Pool(typeof(EventCardPool))]
public class CleanKill()
    : HadesAncientsCard(HadesAncient.Artemis, 1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15M, ValueProp.Move),
        new CardsVar(1)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        bool shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab").Execute(choiceContext);
        if (!shouldTriggerFatal || !attackCommand.Results
                .SelectMany(r => r)
                .Any((Func<DamageResult, bool>)(r => r.WasTargetKilled)))
            return;
        CardModel? card = Owner.RunState.Rng.Niche.NextItem(PileType.Deck.GetPile(Owner).Cards
            .Where(c => c.IsUpgradable));
        if (card == null)
            return;

        using (HadesAncients_CombatManager_IsEnding_Patch.Bypass())
        {
            CardCmd.Upgrade(card);
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5M);
    }
}