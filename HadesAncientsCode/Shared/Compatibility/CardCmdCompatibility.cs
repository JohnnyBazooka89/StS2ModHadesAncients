using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public class CardCmdCompatibility
{
    private static readonly MethodInfo ExhaustMethod =
        typeof(CardCmd).GetMethod(
            nameof(CardCmd.Exhaust),
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types:
            [
                typeof(PlayerChoiceContext),
                typeof(CardModel),
                typeof(bool),
                typeof(bool)
            ],
            modifiers: null)
        ?? throw new MissingMethodException(typeof(CardCmd).FullName, nameof(CardCmd.Exhaust));

    public static async Task Exhaust(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal = false,
        bool skipVisuals = false)
    {
        object? result = ExhaustMethod.Invoke(
            null,
            [choiceContext, card, causedByEthereal, skipVisuals]);

        await (Task)result!;
    }
}