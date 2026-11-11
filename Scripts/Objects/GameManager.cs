using Godot;

// Главный менеджер игры. Висит на корне сцены GameField.
// Хранит металл, следит за башнями и врагами, ловит конец игры
public partial class GameManager : Node3D
{
    // Единственный экземпляр, доступен из других скриптов
    public static GameManager Instance { get; private set; }

    // Сколько металла в начале игры
    [Export] public int StartMetal = 100;
    // Сколько секунд показывается сообщение
    [Export] public float MessageDuration = 1.5f;

    public int Metal { get; private set; }
    public bool IsGameOver { get; private set; }

    private Label _metalLabel;
    private Label _healthLabel;
    private Label _buildLabel;
    private Label _messageLabel;
    private Label _gameOverLabel;
    private float _messageTime = 0.0f;
    private float _playerHealth = 0.0f;
    private float _coreHealth = 0.0f;

    public override void _Ready()
    {
        Instance = this;
        Metal = StartMetal;

        _metalLabel = GetNode<Label>("HUD/MetalLabel");
        _healthLabel = GetNode<Label>("HUD/HealthLabel");
        _buildLabel = GetNode<Label>("HUD/BuildLabel");
        _messageLabel = GetNode<Label>("HUD/MessageLabel");
        _gameOverLabel = GetNode<Label>("HUD/GameOverLabel");
        _messageLabel.Text = "";
        _buildLabel.Text = "";
        _gameOverLabel.Visible = false;

        // Следим за ядром: если оно погибло, игра заканчивается
        Node core = GetTree().GetFirstNodeInGroup("Core");
        HealthComponent coreHealth = core.GetNode<HealthComponent>("HealthComponent");
        _coreHealth = coreHealth.CurrentHealth;
        coreHealth.HealthChanged += hp => { _coreHealth = hp; UpdateHud(); };
        coreHealth.Died += () => EndGame("ядро уничтожено");

        // Следим за игроком: если он погиб, игра тоже заканчивается
        Node player = GetTree().GetFirstNodeInGroup("Player");
        HealthComponent playerHealth = player.GetNode<HealthComponent>("HealthComponent");
        _playerHealth = playerHealth.CurrentHealth;
        playerHealth.HealthChanged += hp => { _playerHealth = hp; UpdateHud(); };
        playerHealth.Died += () => EndGame("игрок погиб");

        // Берем под наблюдение башни и врагов, которые уже стоят на сцене
        foreach (Node enemy in GetTree().GetNodesInGroup("Enemy")) TrackNode(enemy);
        foreach (Node tower in GetTree().GetNodesInGroup("Tower")) TrackNode(tower);

        // И всех, кто появится позже
        GetTree().NodeAdded += OnNodeAdded;

        UpdateHud();
    }

    public override void _ExitTree()
    {
        GetTree().NodeAdded -= OnNodeAdded;
        if (Instance == this) Instance = null;
    }

    private void OnNodeAdded(Node node)
    {
        if (node.IsInGroup("Enemy") || node.IsInGroup("Tower")) TrackNode(node);
    }

    // Подписываемся на удаление объекта со сцены
    private void TrackNode(Node node)
    {
        HealthComponent health = node.GetNodeOrNull<HealthComponent>("HealthComponent");
        if (health == null) return;
        node.TreeExiting += () => OnTrackedNodeRemoved(health);
    }

    // Объект удален со сцены. Если он был уничтожен, даем металл
    private void OnTrackedNodeRemoved(HealthComponent health)
    {
        if (health.IsDead) AddMetal(health.MetalReward);
    }

    public void AddMetal(int amount)
    {
        Metal += amount;
        UpdateHud();
    }

    // Пробуем потратить металл. Возвращает true, если хватило
    public bool TrySpendMetal(int cost)
    {
        if (Metal < cost) return false;
        Metal -= cost;
        UpdateHud();
        return true;
    }

    // Короткое сообщение игроку на экране
    public void ShowMessage(string text)
    {
        _messageLabel.Text = text;
        _messageTime = MessageDuration;
    }

    // Подсказка про постановку башни (пустая строка убирает ее)
    public void SetBuildInfo(string text)
    {
        _buildLabel.Text = text;
    }

    // Конец игры: показываем надпись и ставим игру на паузу
    public void EndGame(string reason)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        _gameOverLabel.Text = "Игра окончена: " + reason + "\nНажмите R для перезапуска";
        _gameOverLabel.Visible = true;
        GetTree().Paused = true;
    }

    public override void _Process(double delta)
    {
        // Убираем сообщение, когда время вышло
        if (_messageTime > 0.0f)
        {
            _messageTime -= (float)delta;
            if (_messageTime <= 0.0f) _messageLabel.Text = "";
        }
    }

    private void UpdateHud()
    {
        _metalLabel.Text = "Металл: " + Metal;
        _healthLabel.Text = "Игрок: " + Mathf.CeilToInt(Mathf.Max(_playerHealth, 0.0f))
            + "   Ядро: " + Mathf.CeilToInt(Mathf.Max(_coreHealth, 0.0f));
    }
}
