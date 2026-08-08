using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HadesAncients.HadesAncientsCode.Ares.Cards;

[Pool(typeof(EventCardPool))]
public class ShiftingForm() : HadesAncientsCard(HadesAncient.Ares, 3, CardType.Power, CardRarity.Token, TargetType.Self)
{
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ICombatState combatState = cardPlay.Player.Creature.CombatState!;

        DemonForm demonForm = combatState.CreateCard<DemonForm>(Owner);
        SerpentForm serpentForm = combatState.CreateCard<SerpentForm>(Owner);
        VoidForm voidForm = combatState.CreateCard<VoidForm>(Owner);
        ReaperForm reaperForm = combatState.CreateCard<ReaperForm>(Owner);
        EchoForm echoForm = combatState.CreateCard<EchoForm>(Owner);

        List<CardModel> formCards = [demonForm, serpentForm, voidForm, reaperForm, echoForm];

        Owner.Creature.CombatState!.RunState.Rng.Niche.Shuffle(formCards);
        
        await CardCmd.AutoPlay(choiceContext, formCards[0], null);
    }
}