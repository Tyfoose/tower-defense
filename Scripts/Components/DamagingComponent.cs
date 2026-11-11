using Godot;

// Компонент нанесения урона. Общая логика для пуль, врагов-ползунов, взрывов и так далее
[GlobalClass]
public partial class DamagingComponent : Area3D
{
    // Сколько урона наносим
    [Export] public float Damage = 10.0f;
    // Группы, которым наносим урон
    [Export] public string[] TargetGroups = new string[0];
    // Удалять ли родителя после попадания в цель
    [Export] public bool DestroyParentOnHit = true;
    // Удалять ли родителя при попадании в стену
    [Export] public bool DestroyOnWorldHit = false;

    // Защита от повторного срабатывания
    private bool _used = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_used) return;

        if (IsTarget(body)) OnTargetHit(body);
        else OnWorldHit();
    }

    // Проверяем по группам, можно ли бить это тело
    protected bool IsTarget(Node3D body)
    {
        foreach (string group in TargetGroups)
        {
            if (body.IsInGroup(group)) return true;
        }
        return false;
    }

    // Что делаем при попадании в цель. Наследники могут это поменять
    protected virtual void OnTargetHit(Node3D target)
    {
        ApplyDamage(target);
        if (DestroyParentOnHit) DestroySelf();
    }

    // Что делаем при попадании в стену. Наследники могут это поменять
    protected virtual void OnWorldHit()
    {
        if (DestroyOnWorldHit) DestroySelf();
    }

    // Наносим урон цели через ее HealthComponent
    public virtual void ApplyDamage(Node3D target)
    {
        HealthComponent health = target.GetNodeOrNull<HealthComponent>("HealthComponent");
        if (health != null) health.TakeDamage(Damage);
    }

    // Самоуничтожение: удаляем весь объект, на котором стоит компонент
    public void DestroySelf()
    {
        _used = true;
        GetParent().QueueFree();
    }
}