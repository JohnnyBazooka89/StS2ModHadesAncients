using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Dionysus.Relics;

[Pool(typeof(EventRelicPool))]
public class StrongDrink() : HadesAncientsRelic(HadesAncient.Dionysus)
{
    private bool _gainStrengthInNextCombat;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(3M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    [SavedProperty]
    private bool GainStrengthInNextCombat
    {
        get => _gainStrengthInNextCombat;
        set
        {
            AssertMutable();
            if (_gainStrengthInNextCombat == value)
                return;
            _gainStrengthInNextCombat = value;
            Status = _gainStrengthInNextCombat ? RelicStatus.Active : RelicStatus.Normal;
        }
    }

    public override IReadOnlyList<LocString> ModifyExtraRestSiteHealText(
        Player player,
        IReadOnlyList<LocString> currentExtraText)
    {
        if (!LocalContext.IsMe(Owner))
        {
            return currentExtraText;
        }

        return [..currentExtraText, AdditionalRestSiteHealText!];
    }

    public override Decimal ModifyRestSiteHealAmount(Creature creature, Decimal amount)
    {
        return creature.Player != Owner && creature.PetOwner != Owner ? amount : creature.MaxHp;
    }

    public override Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        if (player != Owner)
        {
            return Task.CompletedTask;
        }

        Flash();
        GainStrengthInNextCombat = true;
        return Task.CompletedTask;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || !GainStrengthInNextCombat) return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Strength.BaseValue, Owner.Creature, null);
        GainStrengthInNextCombat = false;
    }
}