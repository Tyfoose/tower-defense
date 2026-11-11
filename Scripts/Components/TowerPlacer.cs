using Godot;

// Компонент постановки башен. Вешается на игрока
[GlobalClass]
public partial class TowerPlacer : Node
{
    // Сцены башен, между которыми можно выбирать
    [Export] public Godot.Collections.Array<PackedScene> TowerScenes = new Godot.Collections.Array<PackedScene>();
    // На каком расстоянии перед игроком появляется призрак башни
    [Export] public float PlaceDistance = 3.0f;

    private Node3D _player;
    private Node3D _ghost;
    private int _towerIndex = 0;
    private int _ghostCost = 0;
    private string _ghostName = "";
    private StandardMaterial3D _okMaterial;
    private StandardMaterial3D _badMaterial;

    public override void _Ready()
    {
        _player = GetParent<Node3D>();
        _okMaterial = CreateGhostMaterial(new Color(0.2f, 1.0f, 0.2f, 0.4f));
        _badMaterial = CreateGhostMaterial(new Color(1.0f, 0.2f, 0.2f, 0.4f));
    }

    // Полупрозрачный материал для призрака
    private StandardMaterial3D CreateGhostMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        material.AlbedoColor = color;
        return material;
    }

    public override void _Process(double delta)
    {
        // Клавиша постановки: первое нажатие включает призрак, второе ставит башню
        if (Input.IsActionJustPressed("place_tower"))
        {
            if (_ghost == null) CreateGhost();
            else TryPlaceTower();
        }

        if (_ghost == null) return;

        if (Input.IsActionJustPressed("cancel_build"))
        {
            StopBuilding();
            return;
        }

        // Выбор типа башни
        if (Input.IsActionJustPressed("next_tower"))
        {
            _towerIndex = (_towerIndex + 1) % TowerScenes.Count;
            CreateGhost();
        }

        UpdateGhost();
    }

    // Создаем призрак: копируем только меши башни
    private void CreateGhost()
    {
        if (_ghost != null) _ghost.QueueFree();

        // Создаем башню только чтобы взять у нее меши и цену, в сцену ее не добавляем
        Tower template = TowerScenes[_towerIndex].Instantiate<Tower>();
        _ghostCost = template.Cost;
        _ghostName = template.TowerName;

        _ghost = new Node3D();
        foreach (Node child in template.GetChildren())
        {
            MeshInstance3D mesh = child as MeshInstance3D;
            if (mesh == null) continue;

            MeshInstance3D copy = new MeshInstance3D();
            copy.Mesh = mesh.Mesh;
            copy.Transform = mesh.Transform;
            _ghost.AddChild(copy);
        }
        template.Free();

        GetTree().CurrentScene.AddChild(_ghost);
        GameManager.Instance.SetBuildInfo("Башня: " + _ghostName + "   Цена: " + _ghostCost + "   (E - поставить, Q - сменить, C - отмена)");
    }

    // Двигаем призрак перед игроком и красим по наличию металла
    private void UpdateGhost()
    {
        Vector3 forward = -_player.GlobalTransform.Basis.Z;
        Vector3 position = _player.GlobalPosition + forward * PlaceDistance;
        position.X = Mathf.Round(position.X);
        position.Z = Mathf.Round(position.Z);
        position.Y = 0.0f;
        _ghost.GlobalPosition = position;

        bool canAfford = GameManager.Instance.Metal >= _ghostCost;
        foreach (Node child in _ghost.GetChildren())
        {
            ((MeshInstance3D)child).MaterialOverride = canAfford ? _okMaterial : _badMaterial;
        }
    }

    // Ставим настоящую башню на место призрака
    private void TryPlaceTower()
    {
        if (!GameManager.Instance.TrySpendMetal(_ghostCost))
        {
            GameManager.Instance.ShowMessage("Недостаточно металла!");
            return;
        }

        Node3D tower = TowerScenes[_towerIndex].Instantiate<Node3D>();
        tower.Position = _ghost.GlobalPosition;
        GetTree().CurrentScene.AddChild(tower);
        StopBuilding();
    }

    private void StopBuilding()
    {
        _ghost.QueueFree();
        _ghost = null;
        GameManager.Instance.SetBuildInfo("");
    }
}