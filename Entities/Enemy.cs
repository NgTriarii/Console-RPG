using OOD_Project;
using OOD_Project.Logging;
using OOD_Project.WorldGeneration;
using System;

namespace OOD_Project.Entities;

public abstract class Enemy : OOD_Project.IObserver<SoundEvent>, OOD_Project.IObserver<DeathEvent>
{
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int Attack { get; protected set; }
    public int Armor { get; protected set; }
    public virtual char Symbol { get; protected set; }

    public bool IsDead => Health <= 0;

    public int X { get; set; }
    public int Y { get; set; }
    public IEnemyBehavior CurrentBehavior { get; set; } = new WanderBehavior();

    protected ISubject<SoundEvent> _soundSubject;
    protected ISubject<DeathEvent> _speciesSubject;

    public Enemy(string name, int health, int attack, int armor, char symbol)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Armor = armor;
        Symbol = symbol;
    }

    protected virtual void ReactToSound(int soundX, int soundY) { }

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
    }

    protected override void ReactToSound(int soundX, int soundY)
    {
        CurrentBehavior = new FleeBehavior(soundX, soundY);
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
        CurrentBehavior = new StationaryBehaviour();
    }

    private char _symbol;

    public override char Symbol { get { return Health == 10 ? 'S' : 'M'; } protected set { _symbol = value; } }

    protected override void ReactToSound(int soundX, int soundY)
    {
        CurrentBehavior = (Symbol == 'S') ? new StationaryBehaviour() : new ChaseBehavior(soundX, soundY);
    }

    public override void OnNotify(DeathEvent eventData)
    {
        Attack += 1;
        Health -= 1;
        LogManager.Instance.Log($"The mimic heard about the discovery of its mate! Its form returns to normal!");
        CurrentBehavior = new WanderBehavior();
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
    }

    protected override void ReactToSound(int soundX, int soundY)
    {
        CurrentBehavior = new ChaseBehavior(soundX, soundY);
    }

    public override void OnNotify(DeathEvent eventData)
    {
        Attack += 2;
        Armor += 1;
        LogManager.Instance.Log($"The brawler gets enraged by the death of his brother in arms! Stats increased!");
    }

}
