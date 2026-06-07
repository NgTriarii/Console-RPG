using OOD_Project.Entities;
using OOD_Project.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project;

public class SoundEvent
{
    public int OriginX { get; }
    public int OriginY { get; }
    public int Range { get; }
    public string SourceName { get; }

    private Dictionary<(int X, int Y), int> _reachableTiles;

    public SoundEvent(int originX, int originY, int range, string sourceName, Map map)
    {
        OriginX = originX;
        OriginY = originY;
        Range = range;
        SourceName = sourceName;

        _reachableTiles = CalculateBFS(originX, originY, range, map);
    }

    public bool TryGetHearingDistance(int x, int y, out int distance)
    {
        return _reachableTiles.TryGetValue((x, y), out distance);
    }

    private Dictionary<(int X, int Y), int> CalculateBFS(int startX, int startY, int maxRange, Map map)
    {
        var reachable = new Dictionary<(int X, int Y), int>();
        var queue = new Queue<(int X, int Y, int Dist)>();

        queue.Enqueue((startX, startY, 0));
        reachable[(startX, startY)] = 0;

        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();

            if (curr.Dist >= maxRange) continue;

            for (int i = 0; i < 4; i++)
            {
                int nx = curr.X + dx[i];
                int ny = curr.Y + dy[i];

                if (nx >= 0 && nx < map.Width && ny >= 0 && ny < map.Height)
                {
                    Tile tile = map.Tiles[nx, ny];

                    // Sound doesn't propagate through walls or borders
                    if (!tile.IsWall && !tile.IsBorder)
                    {
                        if (!reachable.ContainsKey((nx, ny)))
                        {
                            reachable[(nx, ny)] = curr.Dist + 1;
                            queue.Enqueue((nx, ny, curr.Dist + 1));
                        }
                    }
                }
            }
        }
        return reachable;
    }
}

public class DeathEvent
{
    public Enemy DeceasedEnemy { get; }

    public DeathEvent(Enemy deceasedEnemy)
    {
        DeceasedEnemy = deceasedEnemy;
    }
}
