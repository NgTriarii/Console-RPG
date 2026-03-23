namespace GameEngine;


public class Game
{
    public string CurrentMessage { get; set; } = "Welcome to the game!";

    public int CursorPos { get; set; } = 0;
    public Map GameMap { get; private set; }
    public Player GamePlayer { get; private set; }
    public Renderer GameRenderer { get; private set; }
    public InputHandler InputChain { get; private set; }

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

    //public void TryMove(int dx, int dy)
    //{
    //    int nextX = _player.X + dx;
    //    int nextY = _player.Y + dy;

    //    if (_map.Tiles[nextX, nextY].IsEnterable)
    //    {
    //        _player.Move(dx, dy, _map.Width, _map.Height);
    //        _map.Tiles[_player.X, _player.Y].OnEntry(_player);
    //    }
    //}

    //public void TryPickUp()
    //{
    //    Tile currentTile = _map.Tiles[_player.X, _player.Y];

    //    if (currentTile.ItemOnTile != null)
    //    { 
    //        currentTile.ItemOnTile.OnPickUp(_player);
    //        currentTile.ItemOnTile = null;
    //    }
    //}

    //public void TryDropItem()
    //{
    //    Tile currentTile = _map.Tiles[_player.X, _player.Y];

    //    if (currentTile.ItemOnTile == null)
    //    {
    //        Item Dropped = _player.DropItem(CursorPos);
    //        _map.Tiles[_player.X, _player.Y].ItemOnTile = Dropped;
    //    }
    //}
    //public void MoveCursor()
    //{
    //    if (CursorPos == _player.Inventory.Count - 1 || CursorPos > _player.Inventory.Count)
    //    {
    //        CursorPos = 0;
    //    }
    //    else CursorPos++;
    //}

    //public void EquipItem()
    //{
    //    if (CursorPos <= _player.Inventory.Count && _player.Inventory.Count != 0)
    //    {
    //        _player.Inventory.Items[CursorPos].Equip(_player);
    //        if (CursorPos == _player.Inventory.Count)
    //        {
    //            CursorPos = 0;
    //        }
    //    }
    //}

    //public void UnequipItem()
    //{
    //    if (_player.RightHand != null)
    //    {
    //        _player.RightHand.Unequip(_player);
    //    }
    //    if (_player.LeftHand != null)
    //    {
    //        _player.LeftHand.Unequip(_player);
    //    }
    //}
    //public void Run()
    //{
    //    Console.CursorVisible = false;
    //    Console.Clear();

    //    Dictionary<ConsoleKey, Action> controls = new Dictionary<ConsoleKey, Action>
    //{
    //    { ConsoleKey.W, () => TryMove(0, -1) },
    //    { ConsoleKey.UpArrow, () => TryMove(0, -1) },
    //    { ConsoleKey.S, () => TryMove(0, 1) },
    //    { ConsoleKey.DownArrow, () => TryMove(0, 1) },
    //    { ConsoleKey.A, () => TryMove(-1, 0) },
    //    { ConsoleKey.LeftArrow, () => TryMove(-1, 0) },
    //    { ConsoleKey.D, () => TryMove(1, 0) },
    //    { ConsoleKey.RightArrow, () => TryMove(1, 0) },

    //    { ConsoleKey.E, () => TryPickUp() },
    //    { ConsoleKey.I, () => MoveCursor() },
    //    { ConsoleKey.R, () => EquipItem() },
    //    { ConsoleKey.T, () => UnequipItem() },
    //    { ConsoleKey.G, () => TryDropItem() }



    //};

    //    while (true)
    //    {
    //        _renderer.DrawFrame(_map, _player, CursorPos);

    //        ConsoleKey key = Console.ReadKey(true).Key;

    //        if (key == ConsoleKey.Escape) break;

    //        if (controls.TryGetValue(key, out Action? action))
    //        {
    //            action.Invoke();
    //        }
    //    }

    //    Console.Clear();
    //    Console.WriteLine("Thanks for playing!");
    //    Console.CursorVisible = true;
    //}

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

        while (true)
        {
            GameRenderer.DrawFrame(GameMap, GamePlayer, CursorPos, CurrentMessage, InputChain);

            ConsoleKey key = Console.ReadKey(true).Key;

            bool wasHandled = InputChain.Handle(key, this);

            if (!wasHandled)
            {
                CurrentMessage = $"[{key}] is not a valid action. Check available controls.";
            }
            else if (!CurrentMessage.Contains("DROP ITEM") && !CurrentMessage.Contains("Equipped") && !CurrentMessage.Contains("Dropped") && !CurrentMessage.Contains("Picked"))
            {
                CurrentMessage = "";
            }
        }
    }
}



