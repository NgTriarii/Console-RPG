using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameEngine;


public class Game
{
    private const int PlayWidth = 40;
    private const int PlayHeight = 20;
    private const int Width = PlayWidth + 2;
    private const int Height = PlayHeight + 2;
    private int CursorPos = 0;


    private Tile[,] _map = new Tile[Width, Height];
    private Player _player = new Player();

    public Game()
    { 
        LoadMap();
    }

    public void LoadMap()
    {

        Random random = new Random();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if ((y == 0 || y == Height - 1))
                {
                    _map[x, y] = new HorizBorder();
                }

                else if ((x == 0 || x == Width - 1))
                {
                    _map[x, y] = new VertBorder();
                }

                //else if (random.Next() % 4 == 0)
                //{
                //    _map[x,y] = new Wall();
                //}

                else
                {
                    _map[x, y] = new Tile();
                }
            }
        }
        GenerateWalls();
        GenerateLoot();
    }

    public void GenerateWalls()
    {
        Random random = new Random();

        for (int x = 1; x < PlayWidth + 1; x++)
        {
            for (int y = 1; y < PlayHeight + 1; y++)
            {
                if (random.Next() % 8 == 0){
                    _map[x, y] = new Wall();
                }
            }
        }

        for (int x = 1; x < PlayWidth + 1; x++)
        {
            for (int y = 1; y < PlayHeight + 1; y++)
            {

                bool hasWallNeighbor = _map[x - 1, y].Symbol == '█' ||
                                   _map[x + 1, y].Symbol == '█' ||
                                   _map[x, y - 1].Symbol == '█' ||
                                   _map[x, y + 1].Symbol == '█';

                if (hasWallNeighbor)
                {
                    if (random.Next() % 4 == 0 && _map[x,y].Symbol != '-' && _map[x, y].Symbol != '|')
                    {
                        _map[x, y] = new Wall();
                    }
                }
            }
        }
    }

    private readonly Func<Item>[] _itemLootTable = new Func<Item>[]
    {
        () => new Gold(),
        () => new Coin(),
        () => new Book(),
        () => new Chalice(),
        () => new Stick(),
        () => new Rapier(),
        () => new Zweihander(),
        () => new Shield()
    };
    private Item GetRandomItem(Random random)
    {
        int roll = random.Next(_itemLootTable.Length);
        return _itemLootTable[roll].Invoke();
    }
    
    public void GenerateLoot()
    {
        Random random = new Random();

        for (int x = 1; x < PlayWidth + 1; x++)
        {
            for (int y = 1; y < PlayHeight + 1; y++)
            {
                if (random.Next() % 20 == 0)
                {
                    _map[x,y].ItemOnTile = GetRandomItem(random);
                }
            }
        }

    }

    private void TryMove(int dx, int dy)
    {
        int nextX = _player.X + dx;
        int nextY = _player.Y + dy;

        if (_map[nextX, nextY].IsEnterable)
        {
            _player.Move(dx, dy, Width, Height);
            _map[_player.X, _player.Y].OnEntry(_player);
        }
    }

    private void TryPickUp()
    {
        Tile currentTile = _map[_player.X, _player.Y];

        if (currentTile.ItemOnTile != null)
        { 
            currentTile.ItemOnTile.OnPickUp(_player);
            currentTile.ItemOnTile = null;
        }
    }

    private void TryDropItem()
    {
        Tile currentTile = _map[_player.X, _player.Y];

        if (currentTile.ItemOnTile == null)
        {
            Item Dropped = _player.DropItem(CursorPos);
            _map[_player.X, _player.Y].ItemOnTile = Dropped;
        }
    }

    public void DrawMap()
    {

        Console.SetCursorPosition(0, 0);

        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < Height; y++)
        {


            for (int x = 0; x < Width; x++)
            {
                if (_player.X == x && _player.Y == y)
                {
                    sb.Append('¶');
                }
                else
                {
                    sb.Append(_map[x, y].Symbol);
                }
            }

            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }

    public void DrawInventory()
    {

        for (int i = 0; i < Height; i++)
        {
            Console.SetCursorPosition(Width, i + 1);
            Console.WriteLine($"                                                         ");
        }

        Console.SetCursorPosition(Width, 0);
        Console.WriteLine("----------Inventory----------");
        Console.SetCursorPosition(Width, 1);
        Console.WriteLine($"Right Hand: {(_player.RightHand != null ? _player.RightHand.Name : ' ')} | Left Hand: {(_player.LeftHand != null ? _player.LeftHand.Name : ' ')} ");
        Console.SetCursorPosition(Width, 2);
        Console.WriteLine($"Coins: {_player.Inventory.Coins} Gold: {_player.Inventory.Gold}");
        for (int i = 0; i < _player.Stats.Count; i++)
        {
            Console.SetCursorPosition(Width, i + 3);
            Console.WriteLine($"{_player.Stats[i].Name} - {_player.Stats[i].Value}");
        }
        Console.SetCursorPosition(Width, _player.Stats.Count + 3);
        Console.WriteLine("Items:");
        for (int i = 0; i < _player.Inventory.Count; i++)
        {
            Console.SetCursorPosition(Width, i + 4 + _player.Stats.Count);
            Console.WriteLine($"-{_player.Inventory.Items[i].Name} {(CursorPos == i ? '<' : ' ')}");

        }
        Console.SetCursorPosition(Width, Height - 1);
        Console.WriteLine("-----------------------------");
    }

    //public void Run()
    //{
    //    Console.CursorVisible = false;
    //    Console.Clear();

    //    bool isRunning = true;

    //    while (isRunning)
    //    {

    //        DrawMap();
    //        DrawInventory();

    //        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

    //        int dx = 0;
    //        int dy = 0;

    //        switch (keyInfo.Key)
    //        {
    //            case ConsoleKey.W:
    //            case ConsoleKey.UpArrow:
    //                dy = -1;
    //                break;
    //            case ConsoleKey.S:
    //            case ConsoleKey.DownArrow:
    //                dy = 1;
    //                break;
    //            case ConsoleKey.A:
    //            case ConsoleKey.LeftArrow:
    //                dx = -1;
    //                break;
    //            case ConsoleKey.D:
    //            case ConsoleKey.RightArrow:
    //                dx = 1;
    //                break;
    //            case ConsoleKey.Escape:
    //                isRunning = false;
    //                break;
    //        }

    //        if ((dx != 0 || dy != 0) && _map[_player.X + dx, _player.Y + dy].IsEnterable == true)
    //        {
    //            _player.Move(dx, dy, Width, Height);
    //            _map[_player.X, _player.Y].OnEntry(_player);
    //        }
    //    }

    //    Console.Clear();
    //    Console.WriteLine("Thanks for playing!");
    //    Console.CursorVisible = true;
    //}

    public void DrawDescription()
    {
        Console.SetCursorPosition(0, Height + 2);

        Console.WriteLine("                                                                                                                ");

        Console.SetCursorPosition(0, Height + 1);
        Console.WriteLine("Tile descrpition:");
        if (_map[_player.X, _player.Y].ItemOnTile != null)
        {
            Console.WriteLine($"{_map[_player.X, _player.Y].ItemOnTile.Name} - {_map[_player.X, _player.Y].ItemOnTile.Description}");
        }
        else Console.WriteLine("An empty tile");
    }

    public void MoveCursor()
    {
        if (CursorPos == _player.Inventory.Count - 1 || CursorPos > _player.Inventory.Count)
        {
            CursorPos = 0;
        }
        else CursorPos++;
    }

    public void EquipItem()
    {
        if (CursorPos <= _player.Inventory.Count && _player.Inventory.Count != 0)
        {
            _player.Inventory.Items[CursorPos].Equip(_player);
            if (CursorPos == _player.Inventory.Count)
            {
                CursorPos = 0;
            }
        }
    }

    public void UnequipItem()
    {
        if (_player.RightHand != null)
        {
            _player.RightHand.Unequip(_player);
        }
        if (_player.LeftHand != null)
        {
            _player.LeftHand.Unequip(_player);
        }
    }
    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        Dictionary<ConsoleKey, Action> controls = new Dictionary<ConsoleKey, Action>
    {
        { ConsoleKey.W, () => TryMove(0, -1) },
        { ConsoleKey.UpArrow, () => TryMove(0, -1) },
        { ConsoleKey.S, () => TryMove(0, 1) },
        { ConsoleKey.DownArrow, () => TryMove(0, 1) },
        { ConsoleKey.A, () => TryMove(-1, 0) },
        { ConsoleKey.LeftArrow, () => TryMove(-1, 0) },
        { ConsoleKey.D, () => TryMove(1, 0) },
        { ConsoleKey.RightArrow, () => TryMove(1, 0) },
        
        { ConsoleKey.E, () => TryPickUp() },
        { ConsoleKey.I, () => MoveCursor() },
        { ConsoleKey.R, () => EquipItem() },
        { ConsoleKey.T, () => UnequipItem() },
        { ConsoleKey.G, () => TryDropItem() }



    };

        while (true)
        {
            DrawMap();
            DrawInventory();
            DrawDescription();

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape) break;

            if (controls.TryGetValue(key, out Action? action))
            {
                action.Invoke();
            }
        }

        Console.Clear();
        Console.WriteLine("Thanks for playing!");
        Console.CursorVisible = true;
    }
}



