using OOD_Project;
using Microsoft.VisualBasic;
using OOD_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Items;
public class Item
{
    public virtual string Name { get; protected set; } = "Default Item";
    public virtual string Description => "A generic item.";

    public virtual int SoundRange { get; } = 0;

    public virtual bool IsEquippable => false;

    public virtual CombatStats AcceptAttack(IAttackAction attack, Player player, Item context = null)
    {
        return attack.ExecuteWithOther(context ?? this, player);
    }

    public virtual void OnPickUp(Player player)
    {
        player.Inventory.Items.Add(this);
    }

    public virtual void Use(Player player)
    {
    }

    public virtual int Equip(Player player, Item context = null)
    {
        return 0;
    }

    public virtual void Unequip(Player player, Item context = null)
    {
    }

}

public class Gold : Item {

    public override string Name => "Gold";
    public override string Description => "A bar of gold.";
    public override void OnPickUp(Player player)
    {
        player.Inventory.Gold++;
    }
}

public class Coin : Item
{
    public override string Name => "Coin";
    public override string Description => "A silver coin. The common currency.";
    public override void OnPickUp(Player player)
    {
        player.Inventory.Coins++;
    }
}

public class Book : Item {
    public override string Name => "Ancient Book";
    public override string Description => "An old and dusty book. Written in an unknown language.";
}

public class Chalice : Item
{
    public override string Name => "Gold Chalice";
    public override string Description => "A beautiful gold chalice. It is suspiciously clean...";

}

public class Stick : Item
{
    public override string Name => "Wooden Stick";
    public override string Description => "A sturdy wooden stick.";
}

public class BareHands : Item
{
    public override string Name => "Bare Hands";
}
