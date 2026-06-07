using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HadesAncients.HadesAncientsCode.Zeus.Cards;

[Pool(typeof(EventCardPool))]
public class AetherFont() : HadesAncientsCard(HadesAncient.Zeus, 0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, CardKeyword.Retain];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];


    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    public override void OnUpgrade() => DynamicVars.Energy.UpgradeValueBy(1);
}