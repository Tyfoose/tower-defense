using Godot;

// Вражеская турель. Едет к ядру, останавливается и стреляет по тем, кого видит
public partial class EnemyTurret : CharacterBody3D
{
    // На каком расстоянии от ядра турель останавливается
    [Export] public float StopDistance = 9.0f;

    private MoveComponent _move;
    private ShootComponent _shoot;
    private VisibilityComponent _visibility;
    private Node3D _core;

    public override void _Ready()
    {
        _move = GetNode<MoveComponent>("MoveComponent");
        _shoot = GetNode<ShootComponent>("ShootComponent");
        _visibility = GetNode<VisibilityComponent>("VisibilityComponent");
        _core = GetTree().GetFirstNodeInGroup("Core") as Node3D;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Если ядра уже нет, просто стоим
        if (!GodotObject.IsInstanceValid(_core))
        {
            _move.Direction = Vector3.Zero;
            return;
        }

        // Едем к ядру, пока не подъехали на нужное расстояние
        Vector3 toCore = _core.GlobalPosition - GlobalPosition;
        toCore.Y = 0.0f;
        if (toCore.Length() > StopDistance) _move.Direction = toCore;
        else _move.Direction = Vector3.Zero;

        // Если видим цель, смотрим на нее и стреляем, иначе смотрим на ядро
        Node3D target = _visibility.GetNearestTarget();
        if (target != null)
        {
            FacePoint(target.GlobalPosition);
            _shoot.TryShoot();
        }
        else
        {
            FacePoint(_core.GlobalPosition);
        }
    }

    private void FacePoint(Vector3 point)
    {
        Vector3 flat = new Vector3(point.X, GlobalPosition.Y, point.Z);
        if (flat.DistanceTo(GlobalPosition) > 0.1f) LookAt(flat, Vector3.Up);
    }
}