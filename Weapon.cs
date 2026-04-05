using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;
public class Weapon : Item
{
    public virtual int Damage { get; set; }

    public virtual bool IsTwoHanded { get; set; } = false;

    public override bool IsEquippable { get; } = true;
}
public class Rapier : Weapon
{
    public override string Name => "Rapier";

    public override string Description => "A light and sharp one-handed rapier.";

    public override int Damage => 5;
    public override void Equip(Player player)
    {
        if(player.RightHand == null) {
        player.Inventory.Items.Remove(this);
        player.RightHand = this;
        }
    }

    public override void Unequip(Player player)
    {
        player.Inventory.Items.Add(this);
        player.RightHand = null;
    }
}

public class Zweihander : Weapon
{
    public override string Name => "Zweihander";

    public override string Description => "A long and cumbersome two-handed zweihander.";

    public override int Damage => 15;

    public override bool IsTwoHanded => true;
    public override void Equip(Player player)
    {
        if (player.RightHand == null && player.LeftHand == null)
        {
            player.Inventory.Items.Remove(this);
            player.RightHand = this;
            player.LeftHand = this;
        }
    }

    public override void Unequip(Player player)
    {
        player.Inventory.Items.Add(this);
        player.RightHand = null;
        player.LeftHand = null;
    }
}

public class Shield : Weapon
{
    public override string Name => "Shield";

    public override string Description => "A round, metal shield with nordic ornamentation.";

    public override int Damage => 0;
    public override void Equip(Player player)
    {
        if (player.LeftHand == null)
        {
            player.Inventory.Items.Remove(this);
            player.LeftHand = this;
        }
    }

    public override void Unequip(Player player)
    {
        player.Inventory.Items.Add(this);
        player.LeftHand = null;
    }
}

