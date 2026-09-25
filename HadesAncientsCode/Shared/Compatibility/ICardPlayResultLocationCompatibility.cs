using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Shared.Compatibility;

public readonly record struct CardLocationCompatibility(
    PileType PileType,
    CardPilePosition Position);

public interface ICardPlayResultLocationCompatibility
{
    CardLocationCompatibility ModifyCardPlayResultLocationCompatibility(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocationCompatibility cardLocation);
}

public static class CardPlayResultLocationCompatibility
{
    private const string OldMethodName =
        "ModifyCardPlayResultPileTypeAndPosition";

    private const string NewMethodName =
        "ModifyCardPlayResultLocation";

    private static readonly Type? CardLocationType =
        typeof(AbstractModel).Assembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == "CardLocation");

    [HarmonyPatch]
    private static class OldVersionPatch
    {
        private static MethodBase? TargetMethod()
        {
            return AccessTools.Method(
                typeof(AbstractModel),
                OldMethodName,
                new[]
                {
                    typeof(CardModel),
                    typeof(bool),
                    typeof(ResourceInfo),
                    typeof(PileType),
                    typeof(CardPilePosition)
                });
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static bool Prefix(
            AbstractModel __instance,
            CardModel card,
            bool isAutoPlay,
            ResourceInfo resources,
            PileType pileType,
            CardPilePosition position,
            ref (PileType, CardPilePosition) __result)
        {
            if (__instance is not ICardPlayResultLocationCompatibility compatibility)
                return true;

            var result = compatibility.ModifyCardPlayResultLocationCompatibility(
                card,
                isAutoPlay,
                resources,
                new CardLocationCompatibility(
                    pileType,
                    position));

            __result = (
                result.PileType,
                result.Position);

            return false;
        }
    }

    [HarmonyPatch]
    private static class NewVersionPatch
    {
        private static FieldInfo? _playerField;
        private static FieldInfo? _pileTypeField;
        private static FieldInfo? _positionField;
        private static ConstructorInfo? _constructor;

        private static MethodBase? TargetMethod()
        {
            if (CardLocationType == null)
                return null;

            return typeof(AbstractModel)
                .GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic)
                .FirstOrDefault(method =>
                {
                    if (method.Name != NewMethodName)
                        return false;

                    var parameters = method.GetParameters();

                    return parameters.Length == 4
                           && parameters[0].ParameterType == typeof(CardModel)
                           && parameters[1].ParameterType == typeof(bool)
                           && parameters[2].ParameterType == typeof(ResourceInfo)
                           && parameters[3].ParameterType == CardLocationType
                           && method.ReturnType == CardLocationType;
                });
        }

        private static bool Prepare()
        {
            if (CardLocationType == null)
                return false;

            _playerField =
                AccessTools.Field(CardLocationType, "player");

            _pileTypeField =
                AccessTools.Field(CardLocationType, "pileType");

            _positionField =
                AccessTools.Field(CardLocationType, "position");

            if (_playerField == null
                || _pileTypeField == null
                || _positionField == null)
            {
                return false;
            }

            _constructor = CardLocationType.GetConstructor(
                new[]
                {
                    _playerField.FieldType,
                    typeof(PileType),
                    typeof(CardPilePosition)
                });

            return _constructor != null
                   && TargetMethod() != null;
        }

        private static void Prefix(
            AbstractModel __instance,
            CardModel card,
            bool isAutoPlay,
            ResourceInfo resources,
            object[] __args)
        {
            if (__instance is not ICardPlayResultLocationCompatibility compatibility)
                return;

            var cardLocation = __args[3];

            var player =
                _playerField!.GetValue(cardLocation)!;

            var pileType =
                (PileType)_pileTypeField!.GetValue(cardLocation)!;

            var position =
                (CardPilePosition)_positionField!.GetValue(cardLocation)!;

            var result = compatibility.ModifyCardPlayResultLocationCompatibility(
                card,
                isAutoPlay,
                resources,
                new CardLocationCompatibility(
                    pileType,
                    position));

            __args[3] = _constructor!.Invoke(
                new[]
                {
                    player,
                    result.PileType,
                    result.Position
                });
        }
    }
}