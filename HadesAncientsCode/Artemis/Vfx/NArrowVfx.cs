using Godot;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HadesAncients.HadesAncientsCode.Artemis.Vfx;

public partial class NArrowVfx : Node2D
{
    public static readonly string scenePath =
        "vfx_support_fire.tscn".VfxPath(HadesAncient.Artemis);

    // How much sideways/upward arc it gets.
    private float ArcHeight = 80f;

    private Node2D? Arrow;

    // How far the projectile initially kicks backwards.
    private float BackwardDistance = 60f;

    private float Duration = 0.25f;

    private Line2D? Trail;

    private async Task Play(Vector2 source, Vector2 target)
    {
        GlobalPosition = Vector2.Zero;

        Arrow!.GlobalPosition = source;

        Trail!.ClearPoints();

        var gradient = new Gradient();

        gradient.Offsets = new[]
        {
            0.0f,
            0.65f,
            1.0f
        };

        gradient.Colors = new[]
        {
            Color.FromHtml("#39FF6A00"),
            Color.FromHtml("#39FF6A"),
            Color.FromHtml("#B8FFD0")
        };

        Trail.Gradient = gradient;

        Trail.Gradient = gradient;

        Trail.AddPoint(source);

        float elapsed = 0.0f;

        Vector2 direction = (target - source).Normalized();
        Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

        Vector2 control1 =
            source
            - direction * BackwardDistance
            + perpendicular * ArcHeight;

        Vector2 control2 =
            source.Lerp(target, 0.55f)
            + perpendicular * ArcHeight * 0.45f;

        Vector2 previousPosition = source;

        while (elapsed < Duration)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            elapsed += (float)GetProcessDeltaTime();

            float t = Mathf.Clamp(elapsed / Duration, 0f, 1f);

            // Starts relatively quickly and accelerates into the target.
            float curvedT = EaseIn(t);

            Vector2 position = CubicBezier(
                source,
                control1,
                control2,
                target,
                curvedT
            );

            Arrow.GlobalPosition = position;

            // Rotate arrow in the direction it is flying.
            Vector2 velocity = position - previousPosition;

            if (velocity.LengthSquared() > 0.001f)
                Arrow.GlobalRotation = velocity.Angle();

            previousPosition = position;

            // Permanent trail for this animation.
            Trail.AddPoint(position);
        }

        Arrow.GlobalPosition = target;

        _ = FinishImpact();
    }

    private static Vector2 CubicBezier(
        Vector2 p0,
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        float t)
    {
        float u = 1.0f - t;

        return
            u * u * u * p0 +
            3f * u * u * t * p1 +
            3f * u * t * t * p2 +
            t * t * t * p3;
    }

    private static float EaseIn(float t)
    {
        // Fast/snappy acceleration into target.
        return t * t;
    }

    private async Task FinishImpact()
    {
        // Keep both the arrow and trail visible after impact.
        await ToSignal(
            GetTree().CreateTimer(0.10f),
            SceneTreeTimer.SignalName.Timeout
        );

        QueueFree();
    }

    public static async Task Create(
        Creature owner,
        Creature target)
    {
        NCreature? ownerNode = owner.GetCreatureNode();
        NCreature? targetNode = target.GetCreatureNode();

        if (ownerNode == null || targetNode == null)
            return;

        NArrowVfx vfx = PreloadManager.Cache
            .GetScene(scenePath)
            .Instantiate<NArrowVfx>();

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);

        await vfx.Play(
            ownerNode.VfxSpawnPosition,
            targetNode.VfxSpawnPosition
        );
    }

    public override void _Ready()
    {
        Arrow = GetNodeOrNull<Node2D>("Arrow")
                ?? throw new InvalidOperationException("Arrow node not found");

        Trail = GetNodeOrNull<Line2D>("Trail")
                ?? throw new InvalidOperationException("Trail node not found");
    }
}