using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hephaestus.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Relics;

[Pool(typeof(EventRelicPool))]
public class TrustyShield() : HadesAncientsRelic(HadesAncient.Hephaestus)
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ForgeArmorPower>(20M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ForgeArmorPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        await PowerCmd.Apply<ForgeArmorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars[nameof(ForgeArmorPower)].BaseValue, Owner.Creature, null);

        Flash();
    }
}