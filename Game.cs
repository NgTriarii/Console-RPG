using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameEngine;


public class Game
{
    private int CursorPos = 0;


    private Map _map;
    private Player _player = new Player();
    private Renderer _renderer = new Renderer();

    public Game()
    {
        MapGenerator generator = new MapGenerator();
        _map = generator.GenerateLevel(40, 20);
    }

    private void TryMove(int dx, int dy)
    {
        int nextX = _player.X + dx;
        int nextY = _player.Y + dy;

        if (_map.Tiles[nextX, nextY].IsEnterable)
        {
            _player.Move(dx, dy, _map.Width, _map.Height);
            _map.Tiles[_player.X, _player.Y].OnEntry(_player);
        }
    }

    private void TryPickUp()
    {
        Tile currentTile = _map.Tiles[_player.X, _player.Y];

        if (currentTile.ItemOnTile != null)
        { 
            currentTile.ItemOnTile.OnPickUp(_player);
            currentTile.ItemOnTile = null;
        }
    }

    private void TryDropItem()
    {
        Tile currentTile = _map.Tiles[_player.X, _player.Y];

        if (currentTile.ItemOnTile == null)
        {
            Item Dropped = _player.DropItem(CursorPos);
            _map.Tiles[_player.X, _player.Y].ItemOnTile = Dropped;
        }
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
            _renderer.DrawFrame(_map, _player, CursorPos);

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



