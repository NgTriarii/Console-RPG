using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace GameEngine;

public class StandardMapBuilder : IMapBuilder
{
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

    private Map _map;
    private Random _random = new Random();

    public bool HasItemsAdded { get; private set; } = false;
    public bool HasWeaponsAdded { get; private set; } = false;

    public void AddLoot(int lootnum)
    {
        Random random = new Random();

        HasItemsAdded = true;
        HasWeaponsAdded = true;

        for (int x = 1; x < _map.PlayWidth + 1; x++)
        {
            for (int y = 1; y < _map.PlayHeight + 1; y++)
            {
                if (random.Next() % 20 == 0)
                {
                    _map.Tiles[x, y].ItemOnTile = GetRandomItem(random);
                }
            }
        }

    }

    private Item GetRandomItem(Random random)
    {
        int roll = random.Next(_itemLootTable.Length);
        return _itemLootTable[roll].Invoke();
    }

    public void StartEmpty(int width, int height)
    {
        _map = new Map(width, height);

        for (int x = 0; x < _map.Width; x++)
        {
            for (int y = 0; y < _map.Height; y++)
            {
                if (x == 0 || x == _map.Width - 1) _map.Tiles[x, y] = new VertBorder();
                else if (y == 0 || y == _map.Height - 1) _map.Tiles[x, y] = new HorizBorder();
                else _map.Tiles[x, y] = new Tile();
            }
        }
    }

    public void StartFilled(int width, int height)
    {
        _map = new Map(width, height);

        for (int x = 0; x < _map.Width; x++)
        {
            for (int y = 0; y < _map.Height; y++)
            {
                if (x == 0 || x == _map.Width - 1) _map.Tiles[x, y] = new VertBorder();
                else if (y == 0 || y == _map.Height - 1) _map.Tiles[x, y] = new HorizBorder();
                else _map.Tiles[x, y] = new Wall();
            }
        }
    }

    public void AddCentralRoom(int roomWidth, int roomHeight)
    {
        int startX = (_map.Width / 2) - (roomWidth / 2);
        int startY = (_map.Height / 2) - (roomHeight / 2);

        for (int x = startX; x < startX + roomWidth; x++)
        {
            for (int y = startY; y < startY + roomHeight; y++)
            {
                if (x > 0 && x < _map.Width - 1 && y > 0 && y < _map.Height - 1)
                    _map.Tiles[x, y] = new Tile();
            }
        }
    }

    public void AddPaths()
    {
        for (int x = 1; x < _map.Width - 1; x++) _map.Tiles[x, _map.Height / 2] = new Tile();
        for (int y = 1; y < _map.Height - 1; y++) _map.Tiles[_map.Width / 2, y] = new Tile();
    }

    public void AddChambers()
    {
        for (int i = 0; i < 3; i++)
        {
            int rx = _random.Next(2, _map.Width - 4);
            int ry = _random.Next(2, _map.Height - 4);
            for (int x = rx; x < rx + 3; x++)
                for (int y = ry; y < ry + 3; y++)
                    _map.Tiles[x, y] = new Tile();
        }
    }

    public void AddItems(int totalItems)
    {
        HasItemsAdded = true;
        PlaceEntities(totalItems, () => new Gold());
    }

    public void AddWeapons(int totalWeapons)
    {
        HasWeaponsAdded = true;
        PlaceEntities(totalWeapons, () => new Stick());
    }

    private void PlaceEntities(int count, Func<Item> itemFactory)
    {
        int placed = 0;
        int attempts = 0;
        int maxAttempts = count * 100;

        while (placed < count && attempts < maxAttempts)
        {
            attempts++;
            int x = _random.Next(1, _map.Width - 1);
            int y = _random.Next(1, _map.Height - 1);

            if (_map.Tiles[x, y] != null && _map.Tiles[x, y].IsEnterable && _map.Tiles[x, y].ItemOnTile == null)
            {
                _map.Tiles[x, y].ItemOnTile = itemFactory();
                placed++;
            }
        }
    }

    public Map GetResult() => _map;
}
