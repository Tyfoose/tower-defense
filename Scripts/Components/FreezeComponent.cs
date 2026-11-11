using Godot;

// Компонент заморозки. Наносит урон и замедляет цель через ее MoveComponent
[GlobalClass]
public partial class FreezeComponent : DamagingComponent
{
    // Во сколько раз остается скорость цели (0.4 значит 40 процентов от обычной)
    [Export] public float SlowMultiplier = 0.4f;
    // Сколько секунд длится замедление
    [Export] public float SlowDuration = 2.0f;

    public override void ApplyDamage(Node3D target)
    {
        base.ApplyDamage(target);

        MoveComponent move = target.GetNodeOrNull<MoveComponent>("MoveComponent");
        if (move != null) move.ApplySlow(SlowMultiplier, SlowDuration);
    }
}