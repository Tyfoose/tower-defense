using Godot;

// Интерфейс. Работает даже на паузе, чтобы можно было перезапустить игру
public partial class Hud : CanvasLayer
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        // После конца игры по кнопке R загружаем сцену заново
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver && Input.IsActionJustPressed("restart"))
        {
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        }
    }
}