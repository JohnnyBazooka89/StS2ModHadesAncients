using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace HadesAncients.HadesAncientsCode.Zeus.Utils;

public class ZeusUtils
{
    public static async Task DealLightningDamage(PlayerChoiceContext choiceContext, Creature dealer, Creature target,
        Decimal damage)
    {
        await Cmd.Wait(0.25f);
        await CreatureCmd.Damage(choiceContext, target, damage, ValueProp.Unpowered, dealer);
        VfxCmd.PlayOnCreature(target, VfxCmd.lightningPath);
    }

    public static async Task DealLightningDamageToAll(PlayerChoiceContext choiceContext, Creature dealer,
        IReadOnlyList<Creature> targets, Decimal damage)
    {
        await Cmd.Wait(0.25f);
        await CreatureCmd.Damage(choiceContext, targets, damage, ValueProp.Unpowered, dealer);
        VfxCmd.PlayOnCreatures(targets, VfxCmd.lightningPath);
    }
}