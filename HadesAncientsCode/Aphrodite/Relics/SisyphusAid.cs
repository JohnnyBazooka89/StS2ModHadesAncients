using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace HadesAncients.HadesAncientsCode.Aphrodite.Relics;

[Pool(typeof(EventRelicPool))]
public class SisyphusAid() : HadesAncientsRelic(HadesAncient.Aphrodite)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<RollingBoulder>()
    ];

    public override async Task AfterAutoPrePlayPhaseEntered(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner || player.Creature.CombatState!.RoundNumber > 1)
        {
            return;
        }

        CardModel rollingBoulderCard = player.Creature.CombatState!.CreateCard<RollingBoulder>(Owner);

        await CardCmd.AutoPlay(choiceContext, rollingBoulderCard, null);
    }
}