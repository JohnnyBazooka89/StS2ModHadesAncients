using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Artemis.Relics;

[Pool(typeof(EventRelicPool))]
public class HunterDash() : HadesAncientsRelic(HadesAncient.Artemis)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(2M),
        new PowerVar<ThornsPower>(3M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<ThornsPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        Flash();
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Dexterity.BaseValue, Owner.Creature, null);
        await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars[nameof(ThornsPower)].BaseValue, Owner.Creature, null);
    }
}