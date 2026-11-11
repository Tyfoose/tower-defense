using Godot;

// Компонент стрельбы. Создает пулю перед родителем и следит за перезарядкой
[GlobalClass]
public partial class ShootComponent : Node
{
    // Сцена пули, которой стреляем
    [Export] public PackedScene BulletScene;
    // Сколько выстрелов в секунду
    [Export] public float ShotsPerSecond = 2.0f;
    // На каком расстоянии перед родителем появляется пуля
    [Export] public float MuzzleDistance = 1.2f;
    // На какой высоте появляется пуля
    [Export] public float MuzzleHeight = 0.5f;

    private Node3D _shooter;
    private float _cooldown = 0.0f;

    public override void _Ready()
    {
        _shooter = GetParent<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_cooldown > 0.0f) _cooldown -= (float)delta;
    }

    // Пробует выстрелить. Если еще идет перезарядка, ничего не делает
    public void TryShoot()
    {
        if (_cooldown > 0.0f) return;
        _cooldown = 1.0f / ShotsPerSecond;

        Node3D bullet = BulletScene.Instantiate<Node3D>();
        GetTree().CurrentScene.AddChild(bullet);

        // Ставим пулю перед стрелком и поворачиваем туда же, куда смотрит стрелок
        Basis basis = _shooter.GlobalTransform.Basis.Orthonormalized();
        Vector3 position = _shooter.GlobalPosition + (-basis.Z) * MuzzleDistance;
        position.Y = MuzzleHeight;
        bullet.GlobalTransform = new Transform3D(basis, position);
    }
}