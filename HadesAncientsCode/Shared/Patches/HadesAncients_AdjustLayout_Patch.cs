using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Godot;
using HadesAncients.HadesAncientsCode.Aphrodite.Ancients;
using HadesAncients.HadesAncientsCode.Athena.Ancients;
using HadesAncients.HadesAncientsCode.Dionysus.Ancients;
using HadesAncients.HadesAncientsCode.Hephaestus.Ancients;
using HadesAncients.HadesAncientsCode.Poseidon.Ancients;
using HadesAncients.HadesAncientsCode.Zeus.Ancients;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch(typeof(NAncientEventLayout), "SetDialogueLineAndAnimate")]
public static class HadesAncients_AdjustLayout_Patch
{
    private const float OriginalSpacing = 10f;

    private const float ReferenceLineBottom = 68f;

    private static readonly IReadOnlyList<LayoutMod> Mods =
    [
        new(
            ancientEvent => ancientEvent.Id == ModelDb.GetId<AthenaAncient>(),
            xOffset: 138f,
            yOffset: 47f,
            scaleAmount: 0.85f
        ),
        new(
            ancientEvent => ancientEvent.Id == ModelDb.GetId<AphroditeAncient>(),
            xOffset: 185f,
            yOffset: -5f,
            scaleAmount: 1.0f
        ),
        new(
            ancientEvent => ancientEvent.Id == ModelDb.GetId<DionysusAncient>(),
            xOffset: 138f,
            yOffset: 47f,
            scaleAmount: 0.85f
        ),
        new(
            ancientEvent => ancientEvent.Id == ModelDb.GetId<HephaestusAncient>(),
            xOffset: 185f,
            yOffset: -5f,
            scaleAmount: 1.0f
        ),
        new(
            ancientEvent => ancientEvent.Id == ModelDb.GetId<PoseidonAncient>(),
            xOffset: 185f,
            yOffset: -5f,
            scaleAmount: 1.0f
        ),
        new(
            ancientEvent => ancientEvent.Id == ModelDb.GetId<ZeusAncient>(),
            xOffset: 170f,
            yOffset: 30f,
            scaleAmount: 0.925f
        ),
    ];

    private static readonly ConditionalWeakTable<NAncientEventLayout, Box> State = new();

    private static bool TryGetLayoutMod(NAncientEventLayout layout, out LayoutMod? mod)
    {
        var ancientEvent = layout._ancientEvent;

        foreach (var candidateMod in Mods)
        {
            if (!candidateMod.IsTargetEvent(ancientEvent))
                continue;

            mod = candidateMod;
            return true;
        }

        mod = null;
        return false;
    }

    static void Prefix(NAncientEventLayout __instance)
    {
        if (!TryGetLayoutMod(__instance, out var mod))
            return;

        var content = __instance._content;
        var contentContainer = __instance._contentContainer;

        if (content == null || contentContainer == null)
            return;

        if (!State.TryGetValue(__instance, out var state))
        {
            state = new Box(content.Position.X, contentContainer.Size.X);
            State.Add(__instance, state);
        }

        content.Position = new Vector2(
            state.BaseContentX + mod!.XOffset,
            content.Position.Y
        );

        content.Scale = new Vector2(mod.ScaleAmount, mod.ScaleAmount);

        contentContainer.ClipContents = false;

        float extraWidth = Mathf.Abs(mod.XOffset) * 2f;

        contentContainer.Size = new Vector2(
            state.BaseContainerWidth + extraWidth,
            contentContainer.Size.Y
        );

        var line = __instance._dialogueContainer.GetChildOrNull<NAncientDialogueLine>(
            __instance._currentDialogueLine
        );

        if (line != null)
        {
            HadesAncientsMainFile.Logger.Warn(
                $"line.Position.Y={line.Position.Y}, " +
                $"line.Size.Y={line.Size.Y}, " +
                $"lineBottom={line.Position.Y + line.Size.Y}, " +
                $"dialogueContainer.Size.Y={__instance._dialogueContainer.Size.Y}"
            );
        }
    }

    private static float GetSpacingForEvent(NAncientEventLayout layout, float originalSpacing)
    {
        if (!TryGetLayoutMod(layout, out var mod))
            return originalSpacing;

        var adjustedYOffset = mod!.YOffset;

        var line = layout._dialogueContainer.GetChildOrNull<NAncientDialogueLine>(
            layout._currentDialogueLine
        );

        if (line != null)
        {
            var lineBottom = line.Position.Y + line.Size.Y;
            var lineDelta = lineBottom - ReferenceLineBottom;

            adjustedYOffset += lineDelta * (1f - mod.ScaleAmount);
        }

        return originalSpacing - adjustedYOffset;
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var targetIndex = -1;

        for (var i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldc_R4 &&
                codes[i].operand is float f &&
                f == OriginalSpacing)
            {
                targetIndex = i;
            }
        }

        for (var i = 0; i < codes.Count; i++)
        {
            if (i == targetIndex)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldc_R4, OriginalSpacing);
                yield return CodeInstruction.Call(
                    typeof(HadesAncients_AdjustLayout_Patch),
                    nameof(GetSpacingForEvent)
                );
            }
            else
            {
                yield return codes[i];
            }
        }
    }

    private sealed class LayoutMod
    {
        public readonly Predicate<AncientEventModel> IsTargetEvent;
        public readonly float ScaleAmount;
        public readonly float XOffset;
        public readonly float YOffset;

        public LayoutMod(
            Predicate<AncientEventModel> isTargetEvent,
            float xOffset,
            float yOffset,
            float scaleAmount
        )
        {
            IsTargetEvent = isTargetEvent;
            XOffset = xOffset;
            YOffset = yOffset;
            ScaleAmount = scaleAmount;
        }
    }

    private sealed class Box
    {
        public readonly float BaseContainerWidth;
        public readonly float BaseContentX;

        public Box(float x, float width)
        {
            BaseContentX = x;
            BaseContainerWidth = width;
        }
    }
}