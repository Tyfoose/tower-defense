using Godot;

// Пуля. Летит вперед через MoveComponent, урон наносит DamagingComponent
public partial class Bullet : CharacterBody3D
{
    // Сколько секунд пуля живет, если ни во что не попала
    [Export] public float Lifetime = 3.0f;

    public override void _PhysicsProcess(double delta)
    {
        Lifetime -= (float)delta;
        if (Lifetime <= 0.0f) QueueFree();
    }
}