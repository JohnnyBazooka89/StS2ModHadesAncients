using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Godot;
using HadesAncients.HadesAncientsCode.Aphrodite.Ancients;
using HadesAncients.HadesAncientsCode.Athena.Ancients;
using HadesAncients.HadesAncientsCode.Dionysus.Ancients;
using HadesAncients.HadesAncientsCode.Hecate.Ancients;
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

    private const float ReferenceThreeButtonHeight = 292f;

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
            ancientEvent => ancientEvent.Id == ModelDb.GetId<HecateAncient>(),
            xOffset: 138f,
            yOffset: 47,
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

    private static readonly ConditionalWeakTable<NAncientEventLayout, LayoutState> State = new();

    private static bool TryGetLayoutMod(NAncientEventLayout layout, out LayoutMod? mod)
    {
        var ancientEvent = layout._ancientEvent;

        if (ancientEvent == null)
        {
            mod = null;
            return false;
        }

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

    private static void Prefix(NAncientEventLayout __instance)
    {
        if (!TryGetLayoutMod(__instance, out var mod))
            return;

        var content = __instance._content;
        var contentContainer = __instance._contentContainer;

        if (content == null || contentContainer == null)
            return;

        var optionCount = __instance.OptionButtons.Count();

        if (!State.TryGetValue(__instance, out var state))
        {
            state = new LayoutState(
                content.Position.X,
                contentContainer.Size.X,
                optionCount
            );

            State.Add(__instance, state);
        }

        if (state.InitialOptionCount == 1 && optionCount == 1)
        {
            var optionsContainer = __instance._optionsContainer;

            optionsContainer.CustomMinimumSize = new Vector2(
                optionsContainer.CustomMinimumSize.X,
                ReferenceThreeButtonHeight
            );

            optionsContainer.Alignment = BoxContainer.AlignmentMode.End;

            optionsContainer.ResetSize();
            content.ResetSize();
        }

        content.Position = new Vector2(
            state.BaseContentX + mod!.XOffset,
            content.Position.Y
        );

        content.Scale = new Vector2(
            mod.ScaleAmount,
            mod.ScaleAmount
        );

        contentContainer.ClipContents = false;

        var extraWidth = Mathf.Abs(mod.XOffset) * 2f;

        contentContainer.Size = new Vector2(
            state.BaseContainerWidth + extraWidth,
            contentContainer.Size.Y
        );
    }

    private static float GetSpacingForEvent(
        NAncientEventLayout layout,
        float originalSpacing
    )
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

        var spacing = originalSpacing - adjustedYOffset;

        /*
         * In the normal flow, the content includes the reference dialogue
         * extent before the three option buttons become a Proceed button.
         *
         * A restored event can start directly with Proceed and no dialogue
         * line. The tween loses the full unscaled extent, while the visible
         * scaled layout loses only extent * scale. Compensate for the
         * difference.
         */
        if (line == null &&
            State.TryGetValue(layout, out var state) &&
            state.InitialOptionCount == 1 &&
            layout.OptionButtons.Count() == 1)
        {
            spacing += ReferenceLineBottom * (1f - mod.ScaleAmount);
        }

        return spacing;
    }

    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions
    )
    {
        var codes = instructions.ToList();

        var targetIndices = codes
            .Select((instruction, index) => (instruction, index))
            .Where(entry =>
                entry.instruction.opcode == OpCodes.Ldc_R4 &&
                entry.instruction.operand is float value &&
                value == OriginalSpacing
            )
            .Select(entry => entry.index)
            .ToList();

        if (targetIndices.Count != 1)
        {
            HadesAncientsMainFile.Logger.Error(
                $"{nameof(HadesAncients_AdjustLayout_Patch)}: " +
                $"expected exactly one {OriginalSpacing}f constant in " +
                $"SetDialogueLineAndAnimate, but found {targetIndices.Count}. " +
                "The spacing adjustment was not applied."
            );

            return codes;
        }

        var targetIndex = targetIndices[0];
        var originalInstruction = codes[targetIndex];

        var loadLayout = new CodeInstruction(OpCodes.Ldarg_0);

        loadLayout.labels.AddRange(originalInstruction.labels);
        loadLayout.blocks.AddRange(originalInstruction.blocks);

        var replacement = new[]
        {
            loadLayout,
            new CodeInstruction(OpCodes.Ldc_R4, OriginalSpacing),
            CodeInstruction.Call(
                typeof(HadesAncients_AdjustLayout_Patch),
                nameof(GetSpacingForEvent)
            ),
        };

        codes.RemoveAt(targetIndex);
        codes.InsertRange(targetIndex, replacement);

        return codes;
    }

    private sealed class LayoutMod(
        Predicate<AncientEventModel> isTargetEvent,
        float xOffset,
        float yOffset,
        float scaleAmount)
    {
        public readonly Predicate<AncientEventModel> IsTargetEvent = isTargetEvent;
        public readonly float ScaleAmount = scaleAmount;
        public readonly float XOffset = xOffset;
        public readonly float YOffset = yOffset;
    }

    private sealed class LayoutState(float x, float width, int initialOptionCount)
    {
        public readonly float BaseContainerWidth = width;
        public readonly float BaseContentX = x;
        public readonly int InitialOptionCount = initialOptionCount;
    }
}