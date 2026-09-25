using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HadesAncients.HadesAncientsCode.Athena.Powers;

public sealed class DivineReprievePower() : HadesAncientsPower(HadesAncient.Athena)
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool IsVisibleInternal => false;

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return Amount > 0 && player == Owner.Player;
    }

    public override async Task AfterTakingExtraTurn(Player player)
    {
        if (player != Owner.Player)
            return;
        await PowerCmd.Decrement(this);
    }
}