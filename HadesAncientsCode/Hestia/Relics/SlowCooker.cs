using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hestia.Enums;
using HadesAncients.HadesAncientsCode.Hestia.RestSiteOptions;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HadesAncients.HadesAncientsCode.Hestia.Relics;

[Pool(typeof(EventRelicPool))]
public class SlowCooker() : HadesAncientsRelic(HadesAncient.Hestia)
{
    private const string CombatStartStrengthKey = "CombatStartStrength";
    private int _combatStartStrength;

    public override bool ShowCounter => true;
    public override int DisplayAmount => CombatStartStrength;

    [SavedProperty]
    private int CombatStartStrength
    {
        get => _combatStartStrength;
        set
        {
            AssertMutable();
            _combatStartStrength = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(SlowCookParams.CardsToRemoveKey, 1),
        new(SlowCookParams.StrengthToGainKey, 1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(HestiaStaticHoverTips.SlowCook, CanonicalVars.ToArray())
    ];

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override Task AfterObtained()
    {
        CombatStartStrength = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner.Creature.IsDead || CombatStartStrength == 0)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            CombatStartStrength, Owner.Creature, null);
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
            return false;
        options.Add(new SlowCookRestSiteOption(player, new SlowCookParams(
            CardsToRemove: DynamicVars[SlowCookParams.CardsToRemoveKey].IntValue,
            StrengthToGain: DynamicVars[SlowCookParams.StrengthToGainKey].IntValue
        )));
        return true;
    }

    public void IncreaseCombatStartStrength()
    {
        CombatStartStrength++;
    }
}