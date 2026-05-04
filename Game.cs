namespace GameEngine;

public class GameConfig
{
    public string PlayerName { get; set; } = "Hero";
    public string LogFilePath { get; set; } = "Logs";
}
public class Game
{
    public string CurrentMessage { get; set; } = "Welcome to the game!";

    public int CursorPos { get; set; } = 0;
    public Map GameMap { get; private set; }
    public Player GamePlayer { get; private set; }
    public Renderer GameRenderer { get; private set; }
    public InputHandler InputChain { get; private set; }

    public GameConfig Config { get; private set; }

    public bool isGameOver { get; set; } = false;
    public bool isLogShown { get; set; } = false;

    public Game()
    {
        //Console.SetWindowSize(200, 200);

        Config = LoadOrCreateConfig("config.ini");

        ILogger fileLogger = new FileLogger(Config.PlayerName, Config.LogFilePath);
        ILogger memoryLogger = new MemoryLogger(100);


        LogManager.Instance.Initialize(fileLogger, memoryLogger);

        LogManager.Instance.Log($"Game started by {Config.PlayerName}.");

        GamePlayer = new Player();
        GameRenderer = new Renderer();

        IMapBuilder mapBuilder = new StandardMapBuilder();
        IInputBuilder inputBuilder = new InputChainBuilder();
        GameDirector director = new GameDirector(mapBuilder, inputBuilder);
        IDungeonTheme theme = new TreasuryTheme();

        director.ConstructThemedDungeon(theme,50, 20);
        //director.ConstructEmptyRoom(40,20);

        GameMap = mapBuilder.GetResult();
        InputChain = inputBuilder.GetResult();

        GamePlayer.X = GameMap.Width / 2;
        GamePlayer.Y = GameMap.Height / 2;

        CurrentMessage = theme.IntroMessage;
    }

    private GameConfig LoadOrCreateConfig(string filePath)
    {
        var config = new GameConfig();

        if (!File.Exists(filePath))
        {
            string[] defaultLines =
            {
                "[Settings]",
                $"PlayerName={config.PlayerName}",
                $"LogFilePath={config.LogFilePath}"
            };
            File.WriteAllLines(filePath, defaultLines);
            return config;
        }

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("[")) continue;

            string[] parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (key.Equals("PlayerName", StringComparison.OrdinalIgnoreCase))
                {
                    config.PlayerName = value;
                }
                else if (key.Equals("LogFilePath", StringComparison.OrdinalIgnoreCase))
                {
                    config.LogFilePath = value;
                }
            }
        }

        return config;
    }

    public void Run()
    {
        
        int requiredWidth = 140;
        int requiredHeight = 90;

        if (Console.BufferWidth < requiredWidth) Console.BufferWidth = requiredWidth;
        if (Console.WindowWidth < requiredWidth) Console.WindowWidth = requiredWidth;

        if (Console.BufferHeight < requiredHeight) Console.BufferHeight = requiredHeight;
        if (Console.WindowHeight < requiredHeight) Console.WindowHeight = requiredHeight;
        

        Console.CursorVisible = false;
        Console.Clear();

        while (!GamePlayer.IsDead && !isGameOver)
        {
            GameRenderer.DrawFrame(GameMap, GamePlayer, CursorPos, CurrentMessage, InputChain);

            if (isLogShown)
            {
                GameRenderer.DrawLog(LogManager.Instance, GameMap.Width, GameMap.Height, 7);
            }

            ConsoleKey key = Console.ReadKey(true).Key;

            bool wasHandled = InputChain.Handle(key, this);

            if (!wasHandled)
            {
                CurrentMessage = $"[{key}] is not a valid action. Check available controls.";
                LogManager.Instance.Log($"Unknown key pressed: {key}");
            }
            //else if (!CurrentMessage.Contains("DROP ITEM") && !CurrentMessage.Contains("Equipped") && !CurrentMessage.Contains("Dropped") && !CurrentMessage.Contains("Picked"))
            //{
            //    CurrentMessage = "";
            //}
        }

        GameRenderer.DrawGameOver();

        LogManager.Instance.Log("Player died. Game Over.");

        ConsoleKey currKey = Console.ReadKey(true).Key;

        while (currKey != ConsoleKey.Enter)
        {
            currKey = Console.ReadKey(true).Key;
        }

    }
}



