using Godot;
using System;

// Компонент здоровья. Вешается на любой объект, который можно уничтожить
[GlobalClass]
public partial class HealthComponent : Node
{
    // Максимальное здоровье
    [Export] public float MaxHealth = 100.0f;
    // Удалять ли родителя со сцены, когда здоровье закончилось
    [Export] public bool DestroyParentOnDeath = true;
    // Сколько металла получит игрок, когда этот объект будет уничтожен
    [Export] public int MetalReward = 0;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    // Событие при изменении здоровья
    public event Action<float> HealthChanged;
    // Событие при смерти
    public event Action Died;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    // Нанесение урона. Вызывается компонентами урона
    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        HealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0.0f)
        {
            IsDead = true;
            Died?.Invoke();
            if (DestroyParentOnDeath) GetParent().QueueFree();
        }
    }
}
