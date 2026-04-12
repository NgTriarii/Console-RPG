using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public abstract class ItemModifier : Item
{
    protected Item _innerItem;

    public ItemModifier(Item innerItem)
    {
        _innerItem = innerItem;
    }
    public override bool IsEquippable => _innerItem.IsEquippable;

    public override int GetBaseDamage(Player player) => _innerItem.GetBaseDamage(player);
    public override int GetLuckBonus() => _innerItem.GetLuckBonus();

    public override CombatStats AcceptAttack(IAttackAction attack, Player player, Item context = null)
    {
        return _innerItem.AcceptAttack(attack, player, context ?? this);
    }

    public override void Equip(Player player, Item context = null)
    {
        _innerItem.Equip(player, context ?? this);
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
    public override int GetBaseDamage(Player player) => base.GetBaseDamage(player) + 5;
}

public class UnluckyModifier : ItemModifier
{
    public UnluckyModifier(Item innerItem) : base(innerItem) { }

    public override string Name => $"{_innerItem.Name} (Unlucky)";
    public override int GetLuckBonus() => base.GetLuckBonus() - 5;
}
