using OOD_Project;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOD_Project.Entities;

public interface IEnemyBehaviour
{
    bool Act(Enemy enemy, GameModel model);
    void OnHearSound(Enemy enemy, int soundX, int soundY);
}

public class BehaviourChain : IEnemyBehaviour
{
    private readonly List<IEnemyBehaviour> _behaviours;

    public BehaviourChain(params IEnemyBehaviour[] behaviours)
    {
        _behaviours = new List<IEnemyBehaviour>(behaviours);
    }

    public bool Act(Enemy enemy, GameModel model)
    {
        foreach (var behaviour in _behaviours)
        {
            if (behaviour.Act(enemy, model)) return true;
        }
        return false;
    }

    public void OnHearSound(Enemy enemy, int soundX, int soundY)
    {
        foreach (var behaviour in _behaviours)
        {
            behaviour.OnHearSound(enemy, soundX, soundY);
        }
    }
}

public class WanderBehaviour : IEnemyBehaviour
{
    private static readonly Random _rng = new Random();

    public bool Act(Enemy enemy, GameModel model)
    {
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        int dir = _rng.Next(4);
        enemy.MoveTo(enemy.X + dx[dir], enemy.Y + dy[dir], model);
        return true;
    }

    public void OnHearSound(Enemy enemy, int soundX, int soundY) { }
}

public class ChaseSoundBehaviour : IEnemyBehaviour
{
    private int _targetX = -1;
    private int _targetY = -1;
    private bool _isActive = false;

    public void OnHearSound(Enemy enemy, int soundX, int soundY)
    {
        _targetX = soundX;
        _targetY = soundY;
        _isActive = true;
    }

    public bool Act(Enemy enemy, GameModel model)
    {
        if (!_isActive) return false;

        if (enemy.X == _targetX && enemy.Y == _targetY)
        {
            _isActive = false;
            return false;
        }

        int dx = Math.Sign(_targetX - enemy.X);
        int dy = Math.Sign(_targetY - enemy.Y);

        if (dx != 0 && enemy.MoveTo(enemy.X + dx, enemy.Y, model)) return true;
        if (dy != 0 && enemy.MoveTo(enemy.X, enemy.Y + dy, model)) return true;

        return true;
    }
}

public class FleeSoundBehaviour : IEnemyBehaviour
{
    private int _soundX = -1;
    private int _soundY = -1;
    private bool _isActive = false;

    public void OnHearSound(Enemy enemy, int soundX, int soundY)
    {
        _soundX = soundX;
        _soundY = soundY;
        _isActive = true;
    }

    public bool Act(Enemy enemy, GameModel model)
    {
        if (!_isActive) return false;

        int dx = Math.Sign(enemy.X - _soundX);
        int dy = Math.Sign(enemy.Y - _soundY);

        bool moved = false;
        if (dx != 0 && enemy.MoveTo(enemy.X + dx, enemy.Y, model)) moved = true;
        else if (dy != 0 && enemy.MoveTo(enemy.X, enemy.Y + dy, model)) moved = true;

        if (!moved)
        {
            _isActive = false;
            return false;
        }

        return true;
    }
}

public class ChasePlayerBehaviour : IEnemyBehaviour
{
    public void OnHearSound(Enemy enemy, int soundX, int soundY) { }

    public bool Act(Enemy enemy, GameModel model)
    {
        var players = enemy.GetPlayersInLineOfSight(model);
        if (players.Count == 0) return false;

        Player target = null;
        int minDistance = int.MaxValue;
        foreach (var p in players)
        {
            int dist = Math.Abs(p.X - enemy.X) + Math.Abs(p.Y - enemy.Y);
            if (dist < minDistance)
            {
                minDistance = dist;
                target = p;
            }
        }

        if (target != null)
        {
            int dx = Math.Sign(target.X - enemy.X);
            int dy = Math.Sign(target.Y - enemy.Y);

            if (dx != 0 && enemy.MoveTo(enemy.X + dx, enemy.Y, model)) return true;
            if (dy != 0 && enemy.MoveTo(enemy.X, enemy.Y + dy, model)) return true;
            
            return true;
        }

        return false;
    }
}

public class FleePlayerBehaviour : IEnemyBehaviour
{
    public void OnHearSound(Enemy enemy, int soundX, int soundY) { }

    public bool Act(Enemy enemy, GameModel model)
    {
        var players = enemy.GetPlayersInLineOfSight(model);
        if (players.Count == 0) return false;

        var forbiddenDirs = new HashSet<(int dx, int dy)>();
        foreach (var p in players)
        {
            if (p.X != enemy.X) forbiddenDirs.Add((Math.Sign(p.X - enemy.X), 0));
            if (p.Y != enemy.Y) forbiddenDirs.Add((0, Math.Sign(p.Y - enemy.Y)));
        }

        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        bool moved = false;
        for (int i = 0; i < 4; i++)
        {
            if (!forbiddenDirs.Contains((dx[i], dy[i])))
            {
                if (enemy.MoveTo(enemy.X + dx[i], enemy.Y + dy[i], model))
                {
                    moved = true;
                    break;
                }
            }
        }

        if (!moved)
        {
            return false;
        }

        return true;
    }
}

public class StationaryBehaviour : IEnemyBehaviour
{
    public bool Act(Enemy enemy, GameModel model)
    {
        return true;
    }

    public void OnHearSound(Enemy enemy, int soundX, int soundY) { }
}
