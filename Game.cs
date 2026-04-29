namespace GameEngine;


public class Game
{
    public string CurrentMessage { get; set; } = "Welcome to the game!";

    public int CursorPos { get; set; } = 0;
    public Map GameMap { get; private set; }
    public Player GamePlayer { get; private set; }
    public Renderer GameRenderer { get; private set; }
    public InputHandler InputChain { get; private set; }

    public bool isGameOver { get; set; } = false;

    public Game()
    {
        GamePlayer = new Player();
        GameRenderer = new Renderer();

        IMapBuilder mapBuilder = new StandardMapBuilder();
        IInputBuilder inputBuilder = new InputChainBuilder();
        GameDirector director = new GameDirector(mapBuilder, inputBuilder);

        director.ConstructDungeon(40, 20);
        //director.ConstructEmptyRoom(40,20);

        GameMap = mapBuilder.GetResult();
        InputChain = inputBuilder.GetResult();

        GamePlayer.X = GameMap.Width / 2;
        GamePlayer.Y = GameMap.Height / 2;
    }

    public void Run()
    {
        
        int requiredWidth = 140;
        int requiredHeight = 40;

        if (Console.BufferWidth < requiredWidth) Console.BufferWidth = requiredWidth;
        if (Console.WindowWidth < requiredWidth) Console.WindowWidth = requiredWidth;

        if (Console.BufferHeight < requiredHeight) Console.BufferHeight = requiredHeight;
        if (Console.WindowHeight < requiredHeight) Console.WindowHeight = requiredHeight;
        

        Console.CursorVisible = false;
        Console.Clear();

        while (!GamePlayer.IsDead)
        {
            GameRenderer.DrawFrame(GameMap, GamePlayer, CursorPos, CurrentMessage, InputChain);

            ConsoleKey key = Console.ReadKey(true).Key;

            bool wasHandled = InputChain.Handle(key, this);

            if (!wasHandled)
            {
                CurrentMessage = $"[{key}] is not a valid action. Check available controls.";
            }
            //else if (!CurrentMessage.Contains("DROP ITEM") && !CurrentMessage.Contains("Equipped") && !CurrentMessage.Contains("Dropped") && !CurrentMessage.Contains("Picked"))
            //{
            //    CurrentMessage = "";
            //}
        }

        GameRenderer.DrawGameOver();

        ConsoleKey currKey = Console.ReadKey(true).Key;

        while (currKey != ConsoleKey.Enter)
        {
            currKey = Console.ReadKey(true).Key;
        }

    }
}



