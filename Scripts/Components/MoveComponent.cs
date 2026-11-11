using Godot;

// Компонент движения. Двигает родителя (CharacterBody3D) в заданном направлении
[GlobalClass]
public partial class MoveComponent : Node
{
    // Скорость движения
    [Export] public float Speed = 5.0f;
    // Если включено, родитель всегда летит вперед (так летят пули)
    [Export] public bool MoveForward = false;

    // Направление, которое задает родитель (игрок или враг)
    public Vector3 Direction = Vector3.Zero;
    // Можно ли сейчас двигаться
    public bool CanMove = true;

    private CharacterBody3D _body;
    private float _speedMultiplier = 1.0f;
    private float _slowTimeLeft = 0.0f;

    public override void _Ready()
    {
        _body = GetParent<CharacterBody3D>();
        // Режим Floating нужен, чтобы тело ездило по плоскости без гравитации
        _body.MotionMode = CharacterBody3D.MotionModeEnum.Floating;
    }

    // Замедление цели. Используется компонентом заморозки
    public void ApplySlow(float multiplier, float duration)
    {
        _speedMultiplier = multiplier;
        _slowTimeLeft = duration;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Отсчитываем время замедления
        if (_slowTimeLeft > 0.0f)
        {
            _slowTimeLeft -= (float)delta;
            if (_slowTimeLeft <= 0.0f) _speedMultiplier = 1.0f;
        }

        Vector3 direction = Direction;
        if (MoveForward) direction = -_body.GlobalTransform.Basis.Z;
        if (!CanMove) direction = Vector3.Zero;

        // Двигаемся только по плоскости
        direction.Y = 0.0f;
        if (direction.LengthSquared() > 0.0f) direction = direction.Normalized();

        _body.Velocity = direction * Speed * _speedMultiplier;
        _body.MoveAndSlide();
    }
}