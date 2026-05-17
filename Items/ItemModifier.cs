using GameEngine;
using OOD_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Items;

public abstract class ItemModifier : Item
{
    protected Item _innerItem;

    public ItemModifier(Item innerItem)
    {
        _innerItem = innerItem;
    }
    public override bool IsEquippable => _innerItem.IsEquippable;

    public override CombatStats AcceptAttack(IAttackAction attack, Player player, Item context = null)
    {
        return _innerItem.AcceptAttack(attack, player, context ?? this);
    }

    public override int Equip(Player player, Item context = null)
    {
        return _innerItem.Equip(player, context ?? this);
    }

    public override void Unequip(Player player, Item context = null)
    {
        _innerItem.Unequip(player, context ?? this);
    }

}

public class StrongModifier : ItemModifier
{
    public StrongModifier(Item innerItem) : base(innerItem) { }

    public override string Name => $"{_innerItem.Name} (Strong)";
    public override int Equip(Player player, Item context = null)
    {

        if (base.Equip(player, context) == 1)
        {
            player.Damage.Value += 5;
            return 1;
        }

        return 0;
    }

    public override void Unequip(Player player, Item context = null)
    {
        base.Unequip(player, context);

        player.Damage.Value -= 5;
    }
}

public class UnluckyModifier : ItemModifier
{
    public UnluckyModifier(Item innerItem) : base(innerItem) { }

    public override string Name => $"{_innerItem.Name} (Unlucky)";

    public override int Equip(Player player, Item context = null)
    {

        if (base.Equip(player, context) == 1)
        {
            player.Luck.Value -= 5;
            return 1;
        }

        return 0;
    }

    public override void Unequip(Player player, Item context = null)
    {
        base.Unequip(player, context);

        player.Luck.Value += 5;
    }
}
