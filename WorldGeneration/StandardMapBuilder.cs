using GameEngine;
using OOD_Project.Entities;
using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OOD_Project.WorldGeneration;

public class StandardMapBuilder : IMapBuilder
{

    private readonly Func<Item, Item>[] _modifierTable = new Func<Item, Item>[]
    {
        (item) => new StrongModifier(item),
        (item) => new UnluckyModifier(item)
    };
    public List<Enemy> SpawnedEnemies { get; private set; } = new List<Enemy>();

    private Map _map;
    private Random _random = new Random();
    private BSPLeaf _rootLeaf;
    private List<BSPLeaf> _leaves = new List<BSPLeaf>();

    public bool HasItemsAdded { get; private set; } = false;
    public bool HasWeaponsAdded { get; private set; } = false;

    public void AddLoot(int totalItems, IDungeonTheme theme)
    {
        int placed = 0, attempts = 0;
        while (placed < totalItems && attempts < totalItems * 100)
        {
            attempts++;
            int x = _random.Next(1, _map.Width - 1);
            int y = _random.Next(1, _map.Height - 1);

            if (_map.Tiles[x, y] != null && _map.Tiles[x, y].IsEnterable && _map.Tiles[x, y].ItemOnTile == null)
            {
                _map.Tiles[x, y].ItemOnTile = theme.GetRandomItem(_random);
                placed++;
            }
        }
    }

    public void AddEnemies(int totalEnemies, IDungeonTheme theme)
    {

        int placed = 0, attempts = 0;
        while (placed < totalEnemies && attempts < totalEnemies * 100)
        {
            attempts++;
            int x = _random.Next(1, _map.Width - 1);
            int y = _random.Next(1, _map.Height - 1);

            if (_map.Tiles[x, y] != null && _map.Tiles[x, y].IsEnterable)
            {
                Enemy newEnemy = theme.GetRandomEnemy(_random);

                newEnemy.X = x;
                newEnemy.Y = y;

                _map.Tiles[x, y].EnemyOnTile = newEnemy;

                SpawnedEnemies.Add(newEnemy);
                placed++;
            }
        }
    }

    public void AddWeapons(int totalItems, IDungeonTheme theme)
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
                _map.Tiles[x, y].ItemOnTile = theme.GetRandomWeapon(_random);
                placed++;
            }
        }
    }

    public void AddModifiedWeapons(int totalItems, IDungeonTheme theme)
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
                
                Item item = theme.GetRandomWeapon(_random);

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

    public void PlaceSpecificItem(Item item)
    {
        int attempts = 0;
        while (attempts < 1000)
        {
            attempts++;
            int x = _random.Next(1, _map.Width - 1);
            int y = _random.Next(1, _map.Height - 1);

            if (_map.Tiles[x, y] != null && _map.Tiles[x, y].IsEnterable && _map.Tiles[x, y].ItemOnTile == null)
            {
                _map.Tiles[x, y].ItemOnTile = item;
                HasItemsAdded = true;
                return;
            }
        }
    }

    public void StartEmpty(int width, int height)
    {
        _leaves.Clear();
        _rootLeaf = null;

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
        _leaves.Clear();
        _rootLeaf = null;

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
        int startX = _map.Width / 2 - roomWidth / 2;
        int startY = _map.Height / 2 - roomHeight / 2;

        if (_rootLeaf == null)
        {
            // CALLED BEFORE ADDCHAMBERS: Manually build the BSP tree around the center
            _rootLeaf = new BSPLeaf(1, 1, _map.Width - 2, _map.Height - 2);

            // Split 1: Isolate the Top region
            _rootLeaf.LeftChild = new BSPLeaf(1, 1, _map.Width - 2, startY - 1);
            _rootLeaf.RightChild = new BSPLeaf(1, startY, _map.Width - 2, _map.Height - 1 - startY);

            var midBottom = _rootLeaf.RightChild;

            // Split 2: Isolate the Bottom region
            midBottom.LeftChild = new BSPLeaf(1, startY, _map.Width - 2, roomHeight); // Middle row
            midBottom.RightChild = new BSPLeaf(1, startY + roomHeight, _map.Width - 2, _map.Height - 1 - (startY + roomHeight));

            var midStrip = midBottom.LeftChild;

            // Split 3: Isolate the Left region
            midStrip.LeftChild = new BSPLeaf(1, startY, startX - 1, roomHeight);
            midStrip.RightChild = new BSPLeaf(startX, startY, _map.Width - 1 - startX, roomHeight);

            var centerRight = midStrip.RightChild;

            // Split 4: Isolate Center and Right regions
            centerRight.LeftChild = new BSPLeaf(startX, startY, roomWidth, roomHeight); // CENTRAL ROOM
            centerRight.RightChild = new BSPLeaf(startX + roomWidth, startY, _map.Width - 1 - (startX + roomWidth), roomHeight);

            // Configure the central leaf so the algorithm treats it as a finished room
            BSPLeaf centralLeaf = centerRight.LeftChild;
            centralLeaf.RoomX = centralLeaf.X;
            centralLeaf.RoomY = centralLeaf.Y;
            centralLeaf.RoomWidth = centralLeaf.Width;
            centralLeaf.RoomHeight = centralLeaf.Height;
            centralLeaf.HasRoom = true; // Protects it from randomization

            // Seed the leaves list for AddChambers to take over
            _leaves.Add(_rootLeaf.LeftChild);  // Top
            _leaves.Add(midBottom.RightChild); // Bottom
            _leaves.Add(midStrip.LeftChild);   // Left
            _leaves.Add(centerRight.RightChild);// Right

            // (We don't draw the tiles here; CreateRooms will handle it perfectly because HasRoom is true).
        }
        else
        {
            // CALLED AFTER ADDCHAMBERS: Carve room and force a corridor
            for (int x = startX; x < startX + roomWidth; x++)
            {
                for (int y = startY; y < startY + roomHeight; y++)
                {
                    if (x > 0 && x < _map.Width - 1 && y > 0 && y < _map.Height - 1)
                        _map.Tiles[x, y] = new Tile();
                }
            }

            var targetNode = _rootLeaf.GetRoomCenter();
            if (targetNode != (0, 0))
            {
                int centerX = startX + roomWidth / 2;
                int centerY = startY + roomHeight / 2;

                int minX = Math.Min(centerX, targetNode.X);
                int maxX = Math.Max(centerX, targetNode.X);
                for (int x = minX; x <= maxX; x++)
                    if (x > 0 && x < _map.Width - 1 && centerY > 0 && centerY < _map.Height - 1) _map.Tiles[x, centerY] = new Tile();

                int minY = Math.Min(centerY, targetNode.Y);
                int maxY = Math.Max(centerY, targetNode.Y);
                for (int y = minY; y <= maxY; y++)
                    if (targetNode.X > 0 && targetNode.X < _map.Width - 1 && y > 0 && y < _map.Height - 1) _map.Tiles[targetNode.X, y] = new Tile();
            }
        }
    }

    public void AddChambers()
    {
        if (_rootLeaf == null)
        {
            _rootLeaf = new BSPLeaf(1, 1, _map.Width - 2, _map.Height - 2);
            _leaves.Add(_rootLeaf);
        }

        bool didSplit = true;

        while (didSplit)
        {
            didSplit = false;

            List<BSPLeaf> currentLeaves = new List<BSPLeaf>(_leaves);
            foreach (var leaf in currentLeaves)
            {
                if (leaf.LeftChild == null && leaf.RightChild == null && !leaf.HasRoom)
                {
                    if (leaf.Width > BSPLeaf.MAX_LEAF_SIZE || leaf.Height > BSPLeaf.MAX_LEAF_SIZE || _random.NextDouble() > 0.25)
                    {
                        if (leaf.Split(_random))
                        {
                            _leaves.Add(leaf.LeftChild);
                            _leaves.Add(leaf.RightChild);
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
                if (!HasRoom) {
                    RoomWidth = random.Next(4, Math.Max(Width - 2,4));
                    RoomHeight = random.Next(4, Math.Max(Height - 2,4));
                    RoomX = random.Next(1, Math.Max(Width - RoomWidth - 1,1)) + X;
                    RoomY = random.Next(1, Math.Max(Height - RoomHeight - 1,1)) + Y;
                    HasRoom = true;
                }

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
