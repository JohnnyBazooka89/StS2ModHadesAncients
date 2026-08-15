using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HadesAncients.HadesAncientsCode.Shared.Utils;

public static class CreatureCmdUtils
{
    public static async Task LoseMaxHpSafely(
        PlayerChoiceContext context,
        Creature creature,
        int requestedMaxHpLoss,
        bool isFromCard)
    {
        var safeMaxHpLoss = Math.Clamp(
            requestedMaxHpLoss,
            0,
            Math.Max(0, creature.MaxHp - 1)
        );

        if (safeMaxHpLoss <= 0)
            return;

        await CreatureCmd.LoseMaxHp(
            context,
            creature,
            safeMaxHpLoss,
            isFromCard
        );
    }
}