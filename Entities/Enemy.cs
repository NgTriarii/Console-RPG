using OOD_Project;
using OOD_Project.Logging;
using OOD_Project.WorldGeneration;
using System;
using System.Collections.Generic;

namespace OOD_Project.Entities;

public abstract class Enemy : OOD_Project.IObserver<SoundEvent>, OOD_Project.IObserver<DeathEvent>
{
    public string Name { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int Health { get; protected set; }
    public int Attack { get; protected set; }
    public int Armor { get; protected set; }
    public virtual char Symbol { get; protected set; }

    public bool IsDead => Health <= 0;

    public int X { get; set; }
    public int Y { get; set; }
    public IEnemyBehaviour CurrentBehaviour { get; set; } = new WanderBehaviour();

    protected ISubject<SoundEvent> _soundSubject;
    protected ISubject<DeathEvent> _speciesSubject;

    public Enemy(string name, int health, int attack, int armor, char symbol)
    {
        Name = name;
        MaxHealth = health;
        Health = health;
        Attack = attack;
        Armor = armor;
        Symbol = symbol;
    }

    protected virtual void ReactToSound(int soundX, int soundY)
    {
        CurrentBehaviour?.OnHearSound(this, soundX, soundY);
    }

    public List<Player> GetPlayersInLineOfSight(GameModel model)
    {
        var players = new List<Player>();
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int dir = 0; dir < 4; dir++)
        {
            for (int dist = 1; dist < Math.Max(model.GameMap.Width, model.GameMap.Height); dist++)
            {
                int checkX = X + dx[dir] * dist;
                int checkY = Y + dy[dir] * dist;

                if (checkX < 0 || checkX >= model.GameMap.Width || checkY < 0 || checkY >= model.GameMap.Height) break;

                Tile tile = model.GameMap.Tiles[checkX, checkY];
                if (tile.IsWall || tile.IsBorder) break;

                Player? p = model.GetPlayerAt(checkX, checkY);
                if (p != null)
                {
                    players.Add(p);
                    break;
                }
            }
        }
        return players;
    }

    public virtual void TakeDamage(int incomingDamage)
    {
        int actualDamage = Math.Max(0, incomingDamage - Armor);
        Health -= actualDamage;

        if (IsDead)
        {
            _speciesSubject?.Notify(new DeathEvent(this));

            _soundSubject?.Detach(this);
            _speciesSubject?.Detach(this);
        }
    }

    protected virtual void PerformAttack(Player player)
    {
        int defense = player.Dexterity.Value;
        int damageDealt = Math.Max(0, Attack - defense);

        player.TakeDamage(damageDealt);

        string msg = $"{Name} attacks you for {damageDealt} damage! (HP: {player.Health.Value})";
        player.LastMessage = msg;
        OOD_Project.Logging.LogManager.Instance.Log(msg);
    }

    public bool MoveTo(int nextX, int nextY, GameModel model)
    {
        Player? targetPlayer = model.GetPlayerAt(nextX, nextY);
        if (targetPlayer != null)
        {
            PerformAttack(targetPlayer);
            return true;
        }

        if (model.GameMap.IsValidMove(nextX, nextY))
        {
            model.GameMap.Tiles[X, Y].EnemyOnTile = null;
            X = nextX;
            Y = nextY;
            model.GameMap.Tiles[X, Y].EnemyOnTile = this;
            return true;
        }
        return false;
    }

    public void RegisterObservers(ISubject<SoundEvent> soundSubject, ISubject<DeathEvent> speciesSubject)
    {
        _soundSubject = soundSubject;
        _speciesSubject = speciesSubject;

        _soundSubject?.Attach(this);
        _speciesSubject?.Attach(this);
    }

    public virtual void OnNotify(SoundEvent eventData)
    {
        if (eventData.TryGetHearingDistance(this.X, this.Y, out int distance))
        {
            LogManager.Instance.Log($"{Name} at ({X}, {Y}) heard {eventData.SourceName} from a distance of {distance}.");
            ReactToSound(eventData.OriginX, eventData.OriginY);
        }
    }
    public virtual void OnNotify(DeathEvent eventData)
    {
    }
}

public class Goblin : Enemy
{
    public Goblin() : base(
        name: "Goblin",
        health: 15,
        attack: 4,
        armor: 1,
        symbol: 'g')
    {
        CurrentBehaviour = new BehaviourChain(new FleePlayerBehaviour(), new FleeSoundBehaviour(), new WanderBehaviour());
    }

    public override void OnNotify(DeathEvent eventData)
    {
        Attack = Math.Max(1, Attack - 1);
        Armor = Math.Max(0, Armor - 1);
        LogManager.Instance.Log($"The creature shivers in fear after hearing about another Goblin's death. Stats decreased!");
    }
}

public class SafeboxMimic : Enemy
{
    public SafeboxMimic() : base(
        name: "SafeboxMimic",
        health: 10,
        attack: 6,
        armor: 0,
        symbol: 'S')
    {
        CurrentBehaviour = new StationaryBehaviour();
    }

    private char _symbol;

    public override char Symbol { get { return Health == MaxHealth ? 'S' : 'M'; } protected set { _symbol = value; } }

    public override void TakeDamage(int incomingDamage)
    {
        base.TakeDamage(incomingDamage);

        if (Health >= MaxHealth / 2)
        {
            CurrentBehaviour = new BehaviourChain(new ChasePlayerBehaviour(), new ChaseSoundBehaviour(), new WanderBehaviour());
        }
        else
        {
            CurrentBehaviour = new BehaviourChain(new FleePlayerBehaviour(), new FleeSoundBehaviour(), new WanderBehaviour());
        }
    }

    public override void OnNotify(DeathEvent eventData)
    {
        Attack += 1;
        Health -= 1;
        LogManager.Instance.Log($"The mimic heard about the discovery of its mate! Its form returns to normal!");

        if (Health >= MaxHealth / 2)
        {
            CurrentBehaviour = new BehaviourChain(new ChasePlayerBehaviour(), new ChaseSoundBehaviour(), new WanderBehaviour());
        }
        else
        {
            CurrentBehaviour = new BehaviourChain(new FleePlayerBehaviour(), new FleeSoundBehaviour(), new WanderBehaviour());
        }
    }
}

public class BriefcaseBrawler : Enemy
{
    public BriefcaseBrawler() : base(
        name: "BriefcaseBrawler",
        health: 20,
        attack: 7,
        armor: 3,
        symbol: 'B')
    {
        CurrentBehaviour = new BehaviourChain(new ChasePlayerBehaviour(), new ChaseSoundBehaviour(), new WanderBehaviour());
    }

    public override void OnNotify(DeathEvent eventData)
    {
        Attack += 2;
        Armor += 1;
        LogManager.Instance.Log($"The brawler gets enraged by the death of his brother in arms! Stats increased!");
    }

}
