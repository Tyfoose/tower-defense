using Godot;

// Спавнер врагов. Создает врагов по краям карты через равные промежутки времени
public partial class EnemySpawner : Node3D
{
    // Сцены врагов. Чтобы ползунов было больше, добавь их в список несколько раз
    [Export] public Godot.Collections.Array<PackedScene> EnemyScenes = new Godot.Collections.Array<PackedScene>();
    // Пауза между появлениями врагов в секундах
    [Export] public float SpawnInterval = 3.0f;
    // Расстояние от центра до места появления
    [Export] public float SpawnDistance = 13.0f;

    private float _timer = 1.0f;
    private RandomNumberGenerator _random = new RandomNumberGenerator();

    public override void _Ready()
    {
        _random.Randomize();
    }

    public override void _Process(double delta)
    {
        _timer -= (float)delta;
        if (_timer > 0.0f) return;

        _timer = SpawnInterval;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        PackedScene scene = EnemyScenes[_random.RandiRange(0, EnemyScenes.Count - 1)];
        Node3D enemy = scene.Instantiate<Node3D>();

        // Выбираем сторону карты и точку на ней
        float along = _random.RandfRange(-SpawnDistance, SpawnDistance);
        int side = _random.RandiRange(0, 3);
        Vector3 position = Vector3.Zero;
        if (side == 0) position = new Vector3(along, 0.0f, -SpawnDistance);
        if (side == 1) position = new Vector3(along, 0.0f, SpawnDistance);
        if (side == 2) position = new Vector3(-SpawnDistance, 0.0f, along);
        if (side == 3) position = new Vector3(SpawnDistance, 0.0f, along);

        enemy.Position = position;
        GetParent().AddChild(enemy);
    }
}