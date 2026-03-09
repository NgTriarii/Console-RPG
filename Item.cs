using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;
public class Item
{
    public virtual string Name => "Default Item";
    public virtual string Description => "A generic item.";

    public virtual void OnPickUp(Player player)
    {
        player.Inventory.Items.Add(this);
    }

    public virtual void Use(Player player)
    {
    }

    public virtual void Equip(Player player)
    {
    }

    public virtual void Unequip(Player player)
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