using OOD_Project.Entities;
using OOD_Project.Inputs;
using OOD_Project.Logging;
using OOD_Project.WorldGeneration;

namespace OOD_Project;

// Handles the main game loop and connects the game state to the screen
public class Game
{
    public GameModel Model { get; private set; }

    public Player LocalPlayer { get; private set; }

    public InputHandler InputChain { get; private set; }

    private readonly IGameView _view;
    private readonly IInputSource _input;

    // Visual state for the current screen
    public int CursorPos { get; set; } = 0;
    public bool IsLogShown { get; set; } = false;

    public Game() : this(new ConsoleView(), new ConsoleInputSource())
    {
    }

    // Sets up a new single-player game
    public Game(IGameView view, IInputSource input)
    {
        _view = view;
        _input = input;

        GameConfig config = GameInitializer.LoadOrCreateConfig("config.ini");
        GameInitializer.InitLogging(config);

        var (model, inputChain) = GameInitializer.Build(config);
        Model = model;
        InputChain = inputChain;

        var (spawnX, spawnY) = Model.FindSpawnPoint();
        LocalPlayer = new Player
        {
            Name = config.PlayerName,
            X = spawnX,
            Y = spawnY,
            LastMessage = Model.CurrentMessage
        };
        Model.AddPlayer(LocalPlayer);
    }

    // Sets up the game for an existing server
    public Game(GameModel model, Player localPlayer, InputHandler inputChain, IGameView view, IInputSource input)
    {
        _view = view;
        _input = input;
        Model = model;
        LocalPlayer = localPlayer;
        InputChain = inputChain;
    }

    // Allows the server to update the screen manually
    public void InitView() => _view.Initialize();
    public void RenderFrame() => _view.Render(Model, LocalPlayer, CursorPos, IsLogShown, InputChain);
    public ConsoleKey ReadKey() => _input.ReadKey();
    public void ShowGameOver() => _view.ShowGameOver();

    // Change what is shown on screen without affecting the game state
    public void ToggleLog() => IsLogShown = !IsLogShown;

    public void MoveCursor()
    {
        int count = LocalPlayer.Inventory.Count;
        if (count > 0)
        {
            CursorPos = (CursorPos + 1) % count;
        }
    }

    public void Run()
    {
        _view.Initialize();

        while (!LocalPlayer.IsDead && !Model.IsGameOver)
        {
            _view.Render(Model, LocalPlayer, CursorPos, IsLogShown, InputChain);

            ConsoleKey key = _input.ReadKey();

            bool wasHandled = InputChain.Handle(key, this);

            if (wasHandled)
            {
                Model.AdvanceEnemyTurn();
            }
            else
            {
                LocalPlayer.LastMessage = $"[{key}] is not a valid action. Check available controls.";
                LogManager.Instance.Log($"Unknown key pressed: {key}");
            }
        }

        _view.ShowGameOver();

        LogManager.Instance.Log("Player died. Game Over.");

        ConsoleKey currKey = _input.ReadKey();

        while (currKey != ConsoleKey.Enter)
        {
            currKey = _input.ReadKey();
        }
    }
}
