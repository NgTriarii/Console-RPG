using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Entities;

public interface IEnemyBehavior
{
    void Act(Enemy enemy, Game game);
}

public class WanderBehavior : IEnemyBehavior
{
    private static readonly Random _rng = new Random();

    public void Act(Enemy enemy, Game game)
    {
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        int dir = _rng.Next(4);
        enemy.MoveTo(enemy.X + dx[dir], enemy.Y + dy[dir], game);
    }
}

public class ChaseBehavior : IEnemyBehavior
{
    private readonly int _targetX;
    private readonly int _targetY;

    public ChaseBehavior(int targetX, int targetY)
    {
        _targetX = targetX;
        _targetY = targetY;
    }

    public void Act(Enemy enemy, Game game)
    {
        int dx = Math.Sign(_targetX - enemy.X);
        int dy = Math.Sign(_targetY - enemy.Y);

        if (dx != 0 && enemy.MoveTo(enemy.X + dx, enemy.Y, game)) return;
        if (dy != 0 && enemy.MoveTo(enemy.X, enemy.Y + dy, game)) return;
    }
}

public class FleeBehavior : IEnemyBehavior
{
    private readonly int _dangerX;
    private readonly int _dangerY;

    public FleeBehavior(int dangerX, int dangerY)
    {
        _dangerX = dangerX;
        _dangerY = dangerY;
    }

    public void Act(Enemy enemy, Game game)
    {
        int dx = Math.Sign(enemy.X - _dangerX);
        int dy = Math.Sign(enemy.Y - _dangerY);

        if (dx != 0 && enemy.MoveTo(enemy.X + dx, enemy.Y, game)) return;
        if (dy != 0 && enemy.MoveTo(enemy.X, enemy.Y + dy, game)) return;
    }
}

public class StationaryBehaviour : IEnemyBehavior
{
    public void Act(Enemy enemy, Game game)
    {
        return;
    }
}