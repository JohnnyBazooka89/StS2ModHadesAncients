using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Hephaestus.Powers;

public class ForgeArmorPower() : HadesAncientsPower(HadesAncient.Hephaestus), IModifyHpLostBeforeOstyAfterLate
{
    private int _blockedDamage;
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int BlockedDamage
    {
        get => _blockedDamage;
        set
        {
            AssertMutable();
            _blockedDamage = value;
        }
    }

    public decimal ModifyHpLostBeforeOstyAfterLate(Creature target, decimal amount, ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        int min = (int)Math.Min(amount, Amount);
        BlockedDamage += min;
        return target != Owner ? amount : amount - min;
    }

    public override async Task AfterModifyingHpLostBeforeOsty()
    {
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this,
            -BlockedDamage, null, null);
        BlockedDamage = 0;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        BlockedDamage = 0;
        return Task.CompletedTask;
    }
}