using System.Reflection;
using System.Reflection.Emit;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

namespace HadesAncients.HadesAncientsCode.Hecate.Patches;

[HarmonyPatch(
    typeof(NRelicCollectionCategory),
    nameof(NRelicCollectionCategory.LoadRelics)
)]
internal static class Hecate_RelicCollectionSort_Patch
{
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        bool patched = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (!patched &&
                instruction.opcode == OpCodes.Call &&
                instruction.operand is MethodInfo calledMethod &&
                IsRelicOrderBy(calledMethod))
            {
                Type keyType = calledMethod.GetGenericArguments()[1];
                int parameterCount = calledMethod.GetParameters().Length;

                MethodInfo replacementDefinition;

                if (parameterCount == 2)
                {
                    replacementDefinition = AccessTools.Method(
                        typeof(Hecate_RelicCollectionSort_Patch),
                        nameof(SortRelics)
                    );
                }
                else
                {
                    replacementDefinition = AccessTools.Method(
                        typeof(Hecate_RelicCollectionSort_Patch),
                        nameof(SortRelicsWithComparer)
                    );
                }

                MethodInfo replacement =
                    replacementDefinition.MakeGenericMethod(keyType);

                /*
                 * The stack currently contains:
                 *
                 * Two-argument overload:
                 *   IEnumerable<RelicModel>
                 *   Func<RelicModel, TKey>
                 *
                 * Three-argument overload:
                 *   IEnumerable<RelicModel>
                 *   Func<RelicModel, TKey>
                 *   IComparer<TKey>
                 *
                 * Add the rarity argument and call our replacement.
                 *
                 * arg0 = category instance
                 * arg1 = RelicRarity
                 */
                var loadRarity = new CodeInstruction(OpCodes.Ldarg_1);
                loadRarity.labels.AddRange(instruction.labels);
                loadRarity.blocks.AddRange(instruction.blocks);

                yield return loadRarity;
                yield return new CodeInstruction(OpCodes.Call, replacement);

                patched = true;
                continue;
            }

            yield return instruction;
        }

        if (!patched)
        {
            FileLog.Log(
                "[Hecate] Failed to locate RelicModel OrderBy in " +
                "NRelicCollectionCategory.LoadRelics."
            );
        }
    }

    private static bool IsRelicOrderBy(MethodInfo method)
    {
        if (method.DeclaringType != typeof(Enumerable) ||
            method.Name != nameof(Enumerable.OrderBy) ||
            !method.IsGenericMethod)
        {
            return false;
        }

        Type[] genericArguments = method.GetGenericArguments();

        if (genericArguments.Length != 2 ||
            genericArguments[0] != typeof(RelicModel))
        {
            return false;
        }

        return method.GetParameters().Length is 2 or 3;
    }

    /// <summary>
    ///     Replacement for:
    ///     Enumerable.OrderBy(source, keySelector)
    /// </summary>
    private static IOrderedEnumerable<RelicModel> SortRelics<TKey>(
        IEnumerable<RelicModel> relics,
        Func<RelicModel, TKey> originalKeySelector,
        RelicRarity rarity)
    {
        return SortRelicsWithComparer(
            relics,
            originalKeySelector,
            Comparer<TKey>.Default,
            rarity
        );
    }

    /// <summary>
    ///     Replacement for:
    ///     Enumerable.OrderBy(source, keySelector, comparer)
    /// </summary>
    private static IOrderedEnumerable<RelicModel> SortRelicsWithComparer<TKey>(
        IEnumerable<RelicModel> relics,
        Func<RelicModel, TKey> originalKeySelector,
        IComparer<TKey>? originalComparer,
        RelicRarity rarity)
    {
        originalComparer ??= Comparer<TKey>.Default;

        /*
         * Preserve the exact original base-game OrderBy for every
         * category except Ancient.
         */
        if (rarity != RelicRarity.Ancient)
        {
            return relics.OrderBy(
                originalKeySelector,
                originalComparer
            );
        }

        return relics.OrderBy(
            relic => relic,
            new ArcanaRelicComparer<TKey>(
                originalKeySelector,
                originalComparer
            )
        );
    }

    private sealed class ArcanaRelicComparer<TKey>(
        Func<RelicModel, TKey> originalKeySelector,
        IComparer<TKey> originalComparer) : IComparer<RelicModel>
    {
        public int Compare(RelicModel? left, RelicModel? right)
        {
            if (ReferenceEquals(left, right))
                return 0;

            if (left is null)
                return 1;

            if (right is null)
                return -1;

            bool leftIsArcana = left is IArcanaRelic;
            bool rightIsArcana = right is IArcanaRelic;

            // Arcana relics appear before all other Ancient relics.
            if (leftIsArcana != rightIsArcana)
                return leftIsArcana ? -1 : 1;

            if (left is IArcanaRelic leftArcana &&
                right is IArcanaRelic rightArcana)
            {
                int numberComparison =
                    leftArcana.GetArcanaRelicNumber().CompareTo(
                        rightArcana.GetArcanaRelicNumber()
                    );

                if (numberComparison != 0)
                    return numberComparison;
            }

            /*
             * Non-Arcana relics, and Arcana relics with an equal number,
             * use the exact original base-game ordering.
             */
            return originalComparer.Compare(
                originalKeySelector(left),
                originalKeySelector(right)
            );
        }
    }
}