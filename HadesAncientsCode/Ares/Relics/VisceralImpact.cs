using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Ares.Cards;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Compatibility;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Ares.Relics;

[Pool(typeof(EventRelicPool))]
public class VisceralImpact() : HadesAncientsRelic(HadesAncient.Ares)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            CardModel characterForm = GetCharacterForm();
            return
            [
                HoverTipFactory.FromCard(characterForm)
            ];
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
            return;
        Flash();
        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, null);
    }

    public override async Task AfterAutoPostPlayPhaseEntered(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner || player.Creature.CombatState!.RoundNumber > 1)
        {
            return;
        }

        CardModel formCard = player.Creature.CombatState!.CreateCard(GetCharacterForm(), Owner);

        await CardCmd.AutoPlay(choiceContext, formCard, null);
    }

    private CardModel GetCharacterForm()
    {
        CardModel? form = IsMutable && Owner != null
            ? Owner.Character.CardPool.AllCards.FirstOrDefault(c =>
                c.Id.Entry.EndsWith("_FORM") && c is { Rarity: CardRarity.Rare, Type: CardType.Power })
            : null;

        return form ?? ModelDb.Card<ShiftingForm>();
    }
}