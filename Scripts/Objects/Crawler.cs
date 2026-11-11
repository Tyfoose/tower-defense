using Godot;

// Ползун. Не стреляет, ползет к ядру и бьет при контакте через DamagingComponent
public partial class Crawler : CharacterBody3D
{
    private MoveComponent _move;
    private Node3D _core;

    public override void _Ready()
    {
        _move = GetNode<MoveComponent>("MoveComponent");
        _core = GetTree().GetFirstNodeInGroup("Core") as Node3D;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Если ядра нет, стоим на месте
        if (!GodotObject.IsInstanceValid(_core))
        {
            _move.Direction = Vector3.Zero;
            return;
        }

        // Ползем прямо к ядру и смотрим в его сторону
        Vector3 toCore = _core.GlobalPosition - GlobalPosition;
        toCore.Y = 0.0f;
        _move.Direction = toCore;

        Vector3 flat = new Vector3(_core.GlobalPosition.X, GlobalPosition.Y, _core.GlobalPosition.Z);
        if (flat.DistanceTo(GlobalPosition) > 0.1f) LookAt(flat, Vector3.Up);
    }
}