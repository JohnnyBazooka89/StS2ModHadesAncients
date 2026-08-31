using System.Reflection;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public static class DamageCmdCompatibility
{
    private static readonly MethodInfo FromCardMethod;
    private static readonly bool UsesCardPlay;

    static DamageCmdCompatibility()
    {
        FromCardMethod =
            typeof(AttackCommand).GetMethod(
                "FromCard",
                [typeof(CardModel), typeof(CardPlay)])
            ?? typeof(AttackCommand).GetMethod(
                "FromCard",
                [typeof(CardModel)])
            ?? throw new MissingMethodException(
                "Could not find AttackCommand.FromCard.");

        UsesCardPlay = FromCardMethod.GetParameters().Length == 2;
    }

    public static AttackCommand FromCard(
        AttackCommand command,
        CardModel card,
        CardPlay cardPlay)
    {
        if (UsesCardPlay)
        {
            return (AttackCommand)FromCardMethod.Invoke(
                command,
                [card, cardPlay])!;
        }

        return (AttackCommand)FromCardMethod.Invoke(
            command,
            [card])!;
    }
}