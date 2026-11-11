using Godot;

// Башня. Неподвижна, поворачивается к ближайшему врагу и стреляет
public partial class Tower : StaticBody3D
{
    // Название башни для интерфейса
    [Export] public string TowerName = "Обычная башня";
    // Цена башни в металле
    [Export] public int Cost = 50;

    private VisibilityComponent _visibility;
    private ShootComponent _shoot;

    public override void _Ready()
    {
        _visibility = GetNode<VisibilityComponent>("VisibilityComponent");
        _shoot = GetNode<ShootComponent>("ShootComponent");
    }

    public override void _PhysicsProcess(double delta)
    {
        Node3D target = _visibility.GetNearestTarget();
        if (target == null) return;

        // Поворачиваемся к цели только по горизонтали
        Vector3 aimPoint = new Vector3(target.GlobalPosition.X, GlobalPosition.Y, target.GlobalPosition.Z);
        if (aimPoint.DistanceTo(GlobalPosition) > 0.1f) LookAt(aimPoint, Vector3.Up);

        _shoot.TryShoot();
    }
}