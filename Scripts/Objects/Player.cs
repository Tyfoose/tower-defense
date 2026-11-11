using Godot;

// Игрок. Ходит по WASD, смотрит на мышь, стреляет. Остальное делают компоненты
public partial class Player : CharacterBody3D
{
    private MoveComponent _move;
    private ShootComponent _shoot;

    public override void _Ready()
    {
        _move = GetNode<MoveComponent>("MoveComponent");
        _shoot = GetNode<ShootComponent>("ShootComponent");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Направление движения с клавиатуры
        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        _move.Direction = new Vector3(input.X, 0.0f, input.Y);

        AimAtMouse();

        // Стрельба по ЛКМ или пробелу
        if (Input.IsActionPressed("shoot")) _shoot.TryShoot();
    }

    // Поворачиваем игрока к точке на полу под курсором мыши
    private void AimAtMouse()
    {
        Camera3D camera = GetViewport().GetCamera3D();
        Vector2 mouse = GetViewport().GetMousePosition();
        Vector3 rayOrigin = camera.ProjectRayOrigin(mouse);
        Vector3 rayDirection = camera.ProjectRayNormal(mouse);

        Vector3? hit = new Plane(Vector3.Up, 0.0f).IntersectsRay(rayOrigin, rayDirection);
        if (!hit.HasValue) return;

        Vector3 aimPoint = new Vector3(hit.Value.X, GlobalPosition.Y, hit.Value.Z);
        if (aimPoint.DistanceTo(GlobalPosition) > 0.1f) LookAt(aimPoint, Vector3.Up);
    }
}