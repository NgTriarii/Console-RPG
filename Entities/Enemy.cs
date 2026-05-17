using System;

namespace OOD_Project.Entities;

public abstract class Enemy
{
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int Attack { get; protected set; }
    public int Armor { get; protected set; }
    public virtual char Symbol { get; protected set; }

    public Enemy(string name, int health, int attack, int armor, char symbol)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Armor = armor;
        Symbol = symbol;
    }

    public virtual void TakeDamage(int incomingDamage)
    {
        int actualDamage = Math.Max(0, incomingDamage - Armor);
        Health -= actualDamage;
    }

    public bool IsDead => Health <= 0;
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
    }

    private char _symbol;

    public override char Symbol { get { return Health == 10 ? 'S' : 'M'; } protected set { _symbol = value; } }
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

}