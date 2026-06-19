using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Athena.Relics;

[Pool(typeof(EventRelicPool))]
public class DivineDash() : HadesAncientsRelic(HadesAncient.Athena)
{
    private int _skillsPlayed;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => SkillsPlayed;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(2M),
        new PowerVar<BlurPower>(1M),
        new CardsVar(8)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<BlurPower>(),
    ];

    [SavedProperty]
    private int SkillsPlayed
    {
        get => _skillsPlayed;
        set
        {
            AssertMutable();
            _skillsPlayed = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        Status = SkillsPlayed == DynamicVars.Cards.IntValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        Flash();
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Dexterity.BaseValue, Owner.Creature, null);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress || cardPlay.IsAutoPlay || cardPlay.Card.Owner != Owner ||
            cardPlay.Card.Type != CardType.Skill)
            return;

        ++SkillsPlayed;
        SkillsPlayed %= DynamicVars.Cards.IntValue;
        if (SkillsPlayed == 0)
        {
            await PowerCmd.Apply<BlurPower>(context, Owner.Creature, DynamicVars[nameof(BlurPower)].BaseValue,
                Owner.Creature, null);
        }
    }
}