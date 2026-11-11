using Godot;
using System.Collections.Generic;

// Компонент взрыва. При попадании расширяет свою зону и бьет всех в радиусе
[GlobalClass]
public partial class ExplosionComponent : DamagingComponent
{
    // Начальный радиус зоны
    [Export] public float StartRadius = 0.3f;
    // Радиус взрыва
    [Export] public float ExplosionRadius = 3.5f;
    // За сколько секунд зона вырастает до полного радиуса
    [Export] public float GrowTime = 0.25f;
    // Сколько секунд взрыв держится после роста
    [Export] public float LingerTime = 0.1f;

    private bool _isExploding = false;
    private float _timer = 0.0f;
    private SphereShape3D _sphere;
    private MeshInstance3D _visual;
    private readonly List<Node3D> _damaged = new List<Node3D>();

    public override void _Ready()
    {
        base._Ready();

        // Делаем свою копию формы, чтобы пули не меняли радиус друг у друга
        CollisionShape3D shapeNode = GetNode<CollisionShape3D>("CollisionShape3D");
        _sphere = (SphereShape3D)shapeNode.Shape.Duplicate();
        _sphere.Radius = StartRadius;
        shapeNode.Shape = _sphere;

        _visual = GetNode<MeshInstance3D>("ExplosionVisual");
        _visual.Visible = false;
    }

    // Попали в цель: запускаем взрыв и бьем каждую цель один раз
    protected override void OnTargetHit(Node3D target)
    {
        StartExplosion();

        if (_damaged.Contains(target)) return;
        _damaged.Add(target);
        ApplyDamage(target);
    }

    // Попали в стену: тоже взрываемся
    protected override void OnWorldHit()
    {
        StartExplosion();
    }

    private void StartExplosion()
    {
        if (_isExploding) return;
        _isExploding = true;
        _visual.Visible = true;

        // Останавливаем полет пули
        MoveComponent move = GetParent().GetNodeOrNull<MoveComponent>("MoveComponent");
        if (move != null) move.CanMove = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isExploding) return;

        _timer += (float)delta;
        float progress = Mathf.Clamp(_timer / GrowTime, 0.0f, 1.0f);
        float radius = Mathf.Lerp(StartRadius, ExplosionRadius, progress);

        // Растет и зона урона, и картинка взрыва
        _sphere.Radius = radius;
        _visual.Scale = Vector3.One * radius * 2.0f;

        if (_timer >= GrowTime + LingerTime) DestroySelf();
    }
}