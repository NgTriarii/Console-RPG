using System;

namespace GameEngine;

public abstract class Enemy
{
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int Attack { get; protected set; }
    public int Armor { get; protected set; }
    public char Symbol { get; protected set; }

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