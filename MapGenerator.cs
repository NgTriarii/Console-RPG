namespace GameEngine;

public class MapGenerator
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


    public Map GenerateLevel(int width, int height)
    {
        Map newMap = new Map(width, height);
        LoadMap(newMap);
        GenerateWalls(newMap);
        GenerateLoot(newMap);
        return newMap;
    }

    public void LoadMap(Map map)
    {

        Random random = new Random();

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                if ((y == 0 || y == map.Height - 1))
                {
                    map.Tiles[x, y] = new HorizBorder();
                }

                else if ((x == 0 || x == map.Width - 1))
                {
                    map.Tiles[x, y] = new VertBorder();
                }

                else
                {
                    map.Tiles[x, y] = new Tile();
                }
            }
        }
    }

    public void GenerateLoot(Map map)
    {
        Random random = new Random();

        for (int x = 1; x < map.PlayWidth + 1; x++)
        {
            for (int y = 1; y < map.PlayHeight + 1; y++)
            {
                if (random.Next() % 20 == 0)
                {
                    map.Tiles[x, y].ItemOnTile = GetRandomItem(random);
                }
            }
        }

    }

    private Item GetRandomItem(Random random)
    {
        int roll = random.Next(_itemLootTable.Length);
        return _itemLootTable[roll].Invoke();
    }

    public void GenerateWalls(Map map)
    {
        Random random = new Random();

        for (int x = 1; x < map.PlayWidth + 1; x++)
        {
            for (int y = 1; y < map.PlayHeight + 1; y++)
            {
                if (random.Next() % 8 == 0)
                {
                    map.Tiles[x, y] = new Wall();
                }
            }
        }

        for (int x = 1; x < map.PlayWidth + 1; x++)
        {
            for (int y = 1; y < map.PlayHeight + 1; y++)
            {

                bool hasWallNeighbor = map.Tiles[x - 1, y].Symbol == '█' ||
                                   map.Tiles[x + 1, y].Symbol == '█' ||
                                   map.Tiles[x, y - 1].Symbol == '█' ||
                                   map.Tiles[x, y + 1].Symbol == '█';

                if (hasWallNeighbor)
                {
                    if (random.Next() % 4 == 0 && map.Tiles[x, y].Symbol != '-' && map.Tiles[x, y].Symbol != '|')
                    {
                        map.Tiles[x, y] = new Wall();
                    }
                }
            }
        }
    }
}
