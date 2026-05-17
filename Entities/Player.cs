using GameEngine;
using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Entities;
public class Player
{
    public string? Name { get; set; }
    public int X { get; set; } = 1;
    public int Y { get; set; } = 1;

    public Inventory Inventory { get; set; } = new Inventory();

    public Item? RightHand { get; set; }

    public Item? LeftHand { get; set; }

    // Stats
    public Stat Health { get; private set; } = new Health();
    public Stat Damage { get ; set; } = new Damage();
    public Stat Aggression { get; private set; } = new Aggression();
    public Stat Wisdom { get; private set; } = new Wisdom();
    public Stat Luck { get; private set; } = new Luck();
    public Stat Strength { get; private set; } = new Strength();
    public Stat Dexterity { get; private set; } = new Dexterity();

    public void Move(int dx, int dy, int mapWidth, int mapHeight)
    {

        X = X + dx;
        Y = Y + dy;

    }

    public Item DropItem(int cursor)
    {
        Item DroppedItem = Inventory.Items[cursor];
        Inventory.Items.RemoveAt(cursor);
        return DroppedItem;
    }

    public bool IsDead => Health.Value <= 0;

    public void TakeDamage(int damage)
    {
        Health.Value -= damage;

        if (Health.Value < 0)
        {
            Health.Value = 0;
        }
    }

    private readonly IAttackAction[] _availableAttacks = new IAttackAction[]
    {
        new NormalAttack(),
        new StealthAttack(),
        new MagicalAttack()
    };

    private int _currentAttackIndex = 0;

    public IAttackAction CurrentAttack => _availableAttacks[_currentAttackIndex];

    public void ToggleAttackMode()
    {
        _currentAttackIndex = (_currentAttackIndex + 1) % _availableAttacks.Length;
    }
}