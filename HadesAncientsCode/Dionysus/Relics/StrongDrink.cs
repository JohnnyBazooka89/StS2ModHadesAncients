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
    private const string RelaxationToGainKey = "RelaxationToGain";
    private int _relaxation;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;

    public override int DisplayAmount => Relaxation;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(RelaxationToGainKey, 1M),
        new PowerVar<StrengthPower>(2M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    [SavedProperty]
    private int Relaxation
    {
        get => _relaxation;
        set
        {
            AssertMutable();
            _relaxation = value;
            InvokeDisplayAmountChanged();
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

    public override decimal ModifyRestSiteHealAmount(Creature creature, decimal amount)
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
        Relaxation++;
        return Task.CompletedTask;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Relaxation <= 0) return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            Relaxation * DynamicVars.Strength.BaseValue, Owner.Creature, null);
    }
}