using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Godot;
using HadesAncients.HadesAncientsCode.Aphrodite.Ancients;
using HadesAncients.HadesAncientsCode.Ares.Ancients;
using HadesAncients.HadesAncientsCode.Athena.Ancients;
using HadesAncients.HadesAncientsCode.Dionysus.Ancients;
using HadesAncients.HadesAncientsCode.Hecate.Ancients;
using HadesAncients.HadesAncientsCode.Hephaestus.Ancients;
using HadesAncients.HadesAncientsCode.Hestia.Ancients;
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
            ancientEvent => ancientEvent.Id == ModelDb.GetId<AresAncient>(),
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
            ancientEvent => ancientEvent.Id == ModelDb.GetId<HestiaAncient>(),
            xOffset: 138f,
            yOffset: 47f,
            scaleAmount: 0.85f
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

    private static void Prefix(NAncientEventLayout __instance, int lineIndex)
    {
        if (!TryGetLayoutMod(__instance, out var mod))
            return;

        var content = __instance._content;
        var contentContainer = __instance._contentContainer;

        if (content == null || contentContainer == null)
            return;

        /*
         * This must happen before changing content.Position.X.
         *
         * The original method will see _contentTween == null and skip its own
         * cleanup block, so the cleanup is not performed twice.
         */
        FinishPreviousContentTween(__instance);

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

            optionsContainer.Alignment =
                BoxContainer.AlignmentMode.End;

            optionsContainer.ResetSize();
            content.ResetSize();
        }

        var hasMultipleDialogueLines =
            __instance._dialogue.Count > 1;

        float appliedXOffset;

        if (!hasMultipleDialogueLines)
        {
            // Preserve the original working one-line layout.
            appliedXOffset = mod!.XOffset;
        }
        else if (lineIndex == 0)
        {
            /*
             * The initial multi-line dialogue already has the correct natural
             * horizontal origin. Applying mod.XOffset here would move it too
             * far to the right.
             */
            appliedXOffset = 0f;
        }
        else
        {
            /*
             * From the first click onward, the changed dialogue layout needs
             * the Ancient-specific offset applied twice. The first clicked
             * line also becomes the global-center reference for later lines.
             */
            appliedXOffset = mod!.XOffset * 2f;
        }

        content.Position = new Vector2(
            state.BaseContentX + appliedXOffset,
            content.Position.Y
        );

        content.Scale = new Vector2(
            mod!.ScaleAmount,
            mod.ScaleAmount
        );

        contentContainer.ClipContents = false;

        /*
         * Reserve enough width for the largest provisional offset.
         * Later global-center corrections should be relatively small.
         */
        var maximumXOffset =
            hasMultipleDialogueLines
                ? Mathf.Abs(mod.XOffset * 2f)
                : Mathf.Abs(mod.XOffset);

        var extraWidth = maximumXOffset * 2f;

        contentContainer.Size = new Vector2(
            state.BaseContainerWidth + extraWidth,
            contentContainer.Size.Y
        );
    }

    private static void AlignOptionsForCurrentDialogue(
        NAncientEventLayout layout,
        int lineIndex
    )
    {
        if (layout._dialogue.Count <= 1)
            return;

        if (lineIndex <= 0)
            return;

        if (!State.TryGetValue(layout, out var state))
            return;

        var content = layout._content;

        if (content == null)
            return;

        var currentCenterX =
            GetOptionGlobalCenterX(layout);

        /*
         * The first clicked line is the known-good reference. Prefix has
         * already applied the additional mod.XOffset at this point.
         */
        if (!state.TargetOptionCenterX.HasValue)
        {
            state.TargetOptionCenterX = currentCenterX;
            return;
        }

        var correction =
            state.TargetOptionCenterX.Value -
            currentCenterX;

        /*
         * Position is in the parent coordinate space. Since correction is
         * measured in global coordinates, apply it directly without scaling.
         */
        content.Position = new Vector2(
            content.Position.X + correction,
            content.Position.Y
        );
    }

    private static float GetOptionGlobalCenterX(
        NAncientEventLayout layout
    )
    {
        var optionButton = layout.OptionButtons.FirstOrDefault();

        Control control =
            optionButton != null
                ? optionButton
                : layout._optionsContainer;

        var localCenter = control.Size * 0.5f;

        var globalCenter =
            control.GetGlobalTransformWithCanvas() *
            localCenter;

        return globalCenter.X;
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
            var dialogueBottom = line.Position.Y + line.Size.Y;
            var optionsHeight = layout._optionsContainer.Size.Y;

            var currentFinalExtent =
                dialogueBottom +
                optionsHeight;

            var referenceFinalExtent =
                ReferenceLineBottom +
                ReferenceThreeButtonHeight;

            var extentDelta =
                currentFinalExtent -
                referenceFinalExtent;

            adjustedYOffset += extentDelta * (1f - mod.ScaleAmount);
        }

        var spacing = originalSpacing - adjustedYOffset;

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

        /*
         * Existing vertical-spacing replacement.
         */
        var spacingIndices = codes
            .Select((instruction, index) => (instruction, index))
            .Where(entry =>
                entry.instruction.opcode == OpCodes.Ldc_R4 &&
                entry.instruction.operand is float value &&
                value == OriginalSpacing
            )
            .Select(entry => entry.index)
            .ToList();

        if (spacingIndices.Count == 1)
        {
            var targetIndex = spacingIndices[0];
            var originalInstruction = codes[targetIndex];

            var loadLayout = new CodeInstruction(OpCodes.Ldarg_0);

            loadLayout.labels.AddRange(originalInstruction.labels);
            loadLayout.blocks.AddRange(originalInstruction.blocks);

            codes.RemoveAt(targetIndex);

            codes.InsertRange(
                targetIndex,
                [
                    loadLayout,
                    new CodeInstruction(
                        OpCodes.Ldc_R4,
                        OriginalSpacing
                    ),
                    CodeInstruction.Call(
                        typeof(HadesAncients_AdjustLayout_Patch),
                        nameof(GetSpacingForEvent)
                    ),
                ]
            );
        }
        else
        {
            HadesAncientsMainFile.Logger.Error(
                $"{nameof(HadesAncients_AdjustLayout_Patch)}: " +
                $"expected exactly one {OriginalSpacing}f constant in " +
                $"SetDialogueLineAndAnimate, but found " +
                $"{spacingIndices.Count}. The vertical spacing adjustment " +
                "was not applied."
            );
        }

        /*
         * Insert horizontal alignment after the new tween is assigned, but
         * before TweenProperty captures content.Position.X.
         */
        var createTweenMethod = AccessTools.Method(
            typeof(Node),
            nameof(Node.CreateTween),
            Type.EmptyTypes
        );

        var contentTweenField = AccessTools.Field(
            typeof(NAncientEventLayout),
            nameof(NAncientEventLayout._contentTween)
        );

        var createTweenCallIndex = codes.FindIndex(instruction => instruction.Calls(createTweenMethod)
        );

        var tweenAssignmentIsValid =
            createTweenCallIndex >= 0 &&
            createTweenCallIndex + 1 < codes.Count &&
            codes[createTweenCallIndex + 1].opcode == OpCodes.Stfld &&
            Equals(
                codes[createTweenCallIndex + 1].operand,
                contentTweenField
            );

        if (!tweenAssignmentIsValid)
        {
            HadesAncientsMainFile.Logger.Error(
                $"{nameof(HadesAncients_AdjustLayout_Patch)}: " +
                "could not find the _contentTween CreateTween assignment. " +
                "The dialogue X alignment was not applied."
            );

            return codes;
        }

        var insertionIndex = createTweenCallIndex + 2;

        var loadLayoutForAlignment =
            new CodeInstruction(OpCodes.Ldarg_0);

        /*
         * Preserve any branch targets or exception blocks attached to the
         * instruction that originally followed the assignment.
         */
        if (insertionIndex < codes.Count)
        {
            loadLayoutForAlignment.labels.AddRange(
                codes[insertionIndex].labels
            );

            codes[insertionIndex].labels.Clear();

            loadLayoutForAlignment.blocks.AddRange(
                codes[insertionIndex].blocks
            );

            codes[insertionIndex].blocks.Clear();
        }

        codes.InsertRange(
            insertionIndex,
            [
                loadLayoutForAlignment,
                new CodeInstruction(OpCodes.Ldarg_1),
                CodeInstruction.Call(
                    typeof(HadesAncients_AdjustLayout_Patch),
                    nameof(AlignOptionsForCurrentDialogue)
                ),
            ]
        );

        return codes;
    }

    private static void FinishPreviousContentTween(
        NAncientEventLayout layout
    )
    {
        var tween = layout._contentTween;

        if (tween == null)
            return;

        /*
         * Settle the previous tween before applying our X position.
         *
         * The original method performs the same cleanup, but it does so after
         * the Harmony Prefix. That can overwrite content.Position.X when the
         * player advances the dialogue before the tween has finished.
         */
        tween.Pause();
        tween.CustomStep(1.0);
        tween.Kill();

        layout._contentTween = null;
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

    private sealed class LayoutState(
        float x,
        float width,
        int initialOptionCount)
    {
        public readonly float BaseContainerWidth = width;
        public readonly float BaseContentX = x;
        public readonly int InitialOptionCount = initialOptionCount;
        public float? TargetOptionCenterX;
    }
}