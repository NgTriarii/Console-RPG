using System;
using System.Collections.Generic;

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

    private readonly Func<Item>[] _weaponLootTable = new Func<Item>[]
    {
        () => new Rapier(),
        () => new Zweihander(),
        () => new Shield()
    };

    private readonly Func<Enemy>[] _enemyTable = new Func<Enemy>[]
    {
        () => new Goblin()
    };

    private readonly Func<Item, Item>[] _modifierTable = new Func<Item, Item>[]
    {
        (item) => new StrongModifier(item),
        (item) => new UnluckyModifier(item)
    };

    private Map _map;
    private Random _random = new Random();
    private BSPLeaf _rootLeaf;

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

    public void AddEnemies(int totalEnemies)
    {
        int placed = 0;
        int attempts = 0;
        int maxAttempts = totalEnemies * 100;

        while (placed < totalEnemies && attempts < maxAttempts)
        {
            attempts++;
            int x = _random.Next(1, _map.Width - 1);
            int y = _random.Next(1, _map.Height - 1);

            if (_map.Tiles[x, y] != null && _map.Tiles[x, y].IsEnterable)
            {
                int roll = _random.Next(_enemyTable.Length);
                _map.Tiles[x, y].EnemyOnTile = _enemyTable[roll].Invoke();
                placed++;
            }
        }
    }

    public void AddModifiedLoot(int totalItems)
    {
        HasItemsAdded = true;
        HasWeaponsAdded = true;

        int placed = 0;
        int attempts = 0;
        int maxAttempts = totalItems * 100;

        while (placed < totalItems && attempts < maxAttempts)
        {
            attempts++;
            int x = _random.Next(1, _map.Width - 1);
            int y = _random.Next(1, _map.Height - 1);

            if (_map.Tiles[x, y] != null && _map.Tiles[x, y].IsEnterable && _map.Tiles[x, y].ItemOnTile == null)
            {
                
                Item item = GetRandomWeapon(_random);

                int modCount = _random.Next(1, 3);
                for (int i = 0; i < modCount; i++)
                {
                    int modRoll = _random.Next(_modifierTable.Length);
                    item = _modifierTable[modRoll].Invoke(item);
                }

                _map.Tiles[x, y].ItemOnTile = item;
                placed++;
            }
        }
    }

    private Item GetRandomItem(Random random)
    {
        int roll = random.Next(_itemLootTable.Length);
        return _itemLootTable[roll].Invoke();
    }

    private Item GetRandomWeapon(Random random)
    {
        int roll = random.Next(_weaponLootTable.Length);
        return _weaponLootTable[roll].Invoke();
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

    public void AddChambers()
    {
        _rootLeaf = new BSPLeaf(1, 1, _map.Width - 2, _map.Height - 2);
        List<BSPLeaf> leaves = new List<BSPLeaf>();
        leaves.Add(_rootLeaf);

        bool didSplit = true;

        while (didSplit)
        {
            didSplit = false;

            List<BSPLeaf> currentLeaves = new List<BSPLeaf>(leaves);
            foreach (var leaf in currentLeaves)
            {
                if (leaf.LeftChild == null && leaf.RightChild == null)
                {
                    if (leaf.Width > BSPLeaf.MAX_LEAF_SIZE || leaf.Height > BSPLeaf.MAX_LEAF_SIZE || _random.NextDouble() > 0.25)
                    {
                        if (leaf.Split(_random))
                        {
                            leaves.Add(leaf.LeftChild);
                            leaves.Add(leaf.RightChild);
                            didSplit = true;
                        }
                    }
                }
            }
        }
        _rootLeaf.CreateRooms(_random, _map);
    }

    public void AddPaths()
    {
        if (_rootLeaf != null)
        {
            _rootLeaf.CreateCorridors(_random, _map);
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


    // BSP map generation algorithm based on: https://www.roguebasin.com/index.php/Basic_BSP_Dungeon_generation#Video_Explanation
    private class BSPLeaf
    {
        public int X, Y, Width, Height;
        public BSPLeaf LeftChild, RightChild;

        public int RoomX, RoomY, RoomWidth, RoomHeight;
        public bool HasRoom = false;

        public const int MIN_LEAF_SIZE = 8;
        public const int MAX_LEAF_SIZE = 24;

        public BSPLeaf(int x, int y, int width, int height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public bool Split(Random random)
        {
            if (LeftChild != null || RightChild != null) return false;

            bool splitH = random.NextDouble() > 0.5;
            if (Width > Height && (double)Width / Height >= 1.25) splitH = false;
            else if (Height > Width && (double)Height / Width >= 1.25) splitH = true;

            int max = (splitH ? Height : Width) - MIN_LEAF_SIZE;
            if (max <= MIN_LEAF_SIZE) return false;

            int split = random.Next(MIN_LEAF_SIZE, max);

            if (splitH)
            {
                LeftChild = new BSPLeaf(X, Y, Width, split);
                RightChild = new BSPLeaf(X, Y + split, Width, Height - split);
            }
            else
            {
                LeftChild = new BSPLeaf(X, Y, split, Height);
                RightChild = new BSPLeaf(X + split, Y, Width - split, Height);
            }
            return true;
        }

        public void CreateRooms(Random random, Map map)
        {
            if (LeftChild != null || RightChild != null)
            {
                if (LeftChild != null) LeftChild.CreateRooms(random, map);
                if (RightChild != null) RightChild.CreateRooms(random, map);
            }
            else
            {
                RoomWidth = random.Next(4, Width - 2);
                RoomHeight = random.Next(4, Height - 2);
                RoomX = random.Next(1, Width - RoomWidth - 1) + X;
                RoomY = random.Next(1, Height - RoomHeight - 1) + Y;
                HasRoom = true;

                for (int x = RoomX; x < RoomX + RoomWidth; x++)
                {
                    for (int y = RoomY; y < RoomY + RoomHeight; y++)
                    {
                        if (x > 0 && x < map.Width - 1 && y > 0 && y < map.Height - 1)
                            map.Tiles[x, y] = new Tile();
                    }
                }
            }
        }

        public (int X, int Y) GetRoomCenter()
        {
            if (HasRoom) return (RoomX + RoomWidth / 2, RoomY + RoomHeight / 2);

            (int X, int Y) leftCenter = LeftChild != null ? LeftChild.GetRoomCenter() : (0, 0);
            (int X, int Y) rightCenter = RightChild != null ? RightChild.GetRoomCenter() : (0, 0);

            if (leftCenter != (0, 0) && rightCenter != (0, 0))
                return new Random().Next(2) == 0 ? leftCenter : rightCenter;
            else if (leftCenter != (0, 0))
                return leftCenter;
            else
                return rightCenter;
        }

        public void CreateCorridors(Random random, Map map)
        {
            if (LeftChild != null || RightChild != null)
            {
                if (LeftChild != null) LeftChild.CreateCorridors(random, map);
                if (RightChild != null) RightChild.CreateCorridors(random, map);

                if (LeftChild != null && RightChild != null)
                {
                    var leftCenter = LeftChild.GetRoomCenter();
                    var rightCenter = RightChild.GetRoomCenter();

                    if (leftCenter != (0, 0) && rightCenter != (0, 0))
                    {
                        DrawCorridor(leftCenter, rightCenter, map, random);
                    }
                }
            }
        }

        private void DrawCorridor((int X, int Y) start, (int X, int Y) end, Map map, Random random)
        {
            // L-Shaped corridor
            if (random.Next(2) == 0)
            {
                DrawHorizontal(start.X, end.X, start.Y, map);
                DrawVertical(start.Y, end.Y, end.X, map);
            }
            else
            {
                DrawVertical(start.Y, end.Y, start.X, map);
                DrawHorizontal(start.X, end.X, end.Y, map);
            }
        }

        private void DrawHorizontal(int x1, int x2, int y, Map map)
        {
            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);

            for (int x = startX; x <= endX; x++)
            {
                if (x > 0 && x < map.Width - 1 && y > 0 && y < map.Height - 1)
                    map.Tiles[x, y] = new Tile();
            }
        }

        private void DrawVertical(int y1, int y2, int x, Map map)
        {
            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            for (int y = startY; y <= endY; y++)
            {
                if (x > 0 && x < map.Width - 1 && y > 0 && y < map.Height - 1)
                    map.Tiles[x, y] = new Tile();
            }
        }
    }
}
