using Godot;
using System.Collections.Generic;

// Компонент видимости. Зона обзора, которая запоминает цели из нужных групп
[GlobalClass]
public partial class VisibilityComponent : Area3D
{
    // Группы, которые мы считаем целями
    [Export] public string[] TargetGroups = new string[0];

    private readonly List<Node3D> _visibleTargets = new List<Node3D>();

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        foreach (string group in TargetGroups)
        {
            if (body.IsInGroup(group))
            {
                _visibleTargets.Add(body);
                return;
            }
        }
    }

    private void OnBodyExited(Node3D body)
    {
        _visibleTargets.Remove(body);
    }

    // Возвращает ближайшую цель или null, если целей нет
    public Node3D GetNearestTarget()
    {
        // Убираем цели, которые уже удалены
        _visibleTargets.RemoveAll(t => !GodotObject.IsInstanceValid(t) || t.IsQueuedForDeletion());

        Node3D nearest = null;
        float bestDistance = float.MaxValue;
        foreach (Node3D target in _visibleTargets)
        {
            float distance = GlobalPosition.DistanceSquaredTo(target.GlobalPosition);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = target;
            }
        }
        return nearest;
    }
}