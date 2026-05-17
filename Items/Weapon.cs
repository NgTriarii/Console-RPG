using GameEngine;
using OOD_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Items;
public class Weapon : Item
{
    public virtual int Damage { get; set; }

    public virtual bool IsTwoHanded { get; set; } = false;

    public override bool IsEquippable { get; } = true;
}

public class HeavyWeapon : Weapon
{
    public override bool IsEquippable => true;

    public override CombatStats AcceptAttack(IAttackAction attack, Player player, Item context = null) =>
        attack.ExecuteWithHeavy(context ?? this, player);
}

public class LightWeapon : Weapon
{
    public override bool IsEquippable => true;

    public override CombatStats AcceptAttack(IAttackAction attack, Player player, Item context = null) =>
        attack.ExecuteWithLight(context ?? this, player);
}

public class MagicalWeapon : Weapon
{
    public override bool IsEquippable => true;

    public override CombatStats AcceptAttack(IAttackAction attack, Player player, Item context = null) =>
        attack.ExecuteWithMagical(context ?? this, player);
}
public class Rapier : LightWeapon
{
    public override string Name => "Rapier";

    public override string Description => "A light and sharp one-handed rapier.";

    public override int Damage => 5;
    public override int Equip(Player player, Item context = null)
    {
        Item toEquip = context ?? this;

        if (player.RightHand == null) {
            player.Damage.Value += Damage;
            player.Inventory.Items.Remove(toEquip);
            player.RightHand = toEquip;
            return 1;
        }

        return 0;
    }

    public override void Unequip(Player player, Item context = null)
    {
        Item toUnequip = context ?? this;

        player.Damage.Value -= Damage;

        player.Inventory.Items.Add(toUnequip);
        player.RightHand = null;
    }
}

public class Zweihander : HeavyWeapon
{
    public override string Name => "Zweihander";

    public override string Description => "A long and cumbersome two-handed zweihander.";

    public override int Damage => 15;

    public override bool IsTwoHanded => true;
    public override int Equip(Player player, Item context = null)
    {
        Item toEquip = context ?? this;

        if (player.RightHand == null && player.LeftHand == null)
        {
            player.Damage.Value += Damage;
            player.Inventory.Items.Remove(toEquip);
            player.RightHand = toEquip;
            player.LeftHand = toEquip;
            return 1;
        }

        return 0;
    }

    public override void Unequip(Player player, Item context = null)
    {
        Item toUnequip = context ?? this;

        player.Damage.Value -= Damage;

        player.Inventory.Items.Add(toUnequip);
        player.RightHand = null;
        player.LeftHand = null;
    }
}

public class Shield : HeavyWeapon
{
    public override string Name => "Shield";

    public override string Description => "A round, metal shield with nordic ornamentation.";

    public override int Damage => 0;
    public override int Equip(Player player, Item context = null)
    {
        Item toEquip = context ?? this;

        if (player.LeftHand == null)
        {
            player.Damage.Value += Damage;
            player.Inventory.Items.Remove(toEquip);
            player.LeftHand = toEquip;
            return 1;
        }

        return 0;
    }

    public override void Unequip(Player player, Item context = null)
    {
        Item toUnequip = context ?? this;

        player.Damage.Value -= Damage;

        player.Inventory.Items.Add(toUnequip);
        player.LeftHand = null;
    }
}

public class LuckyCoinPouch : HeavyWeapon
{
    public override string Name => "Lucky Coin Pouch";

    public override string Description => "A heavy bag filled with coins, you somehow feel like things will now go your way...";

    public override int Damage => 14;

    public override bool IsTwoHanded => true;
    public override int Equip(Player player, Item context = null)
    {
        Item toEquip = context ?? this;

        if (player.RightHand == null && player.LeftHand == null)
        {
            player.Damage.Value += Damage;
            player.Luck.Value += 7;

            player.Inventory.Items.Remove(toEquip);
            player.RightHand = toEquip;
            player.LeftHand = toEquip;
            return 1;
        }

        return 0;
    }

    public override void Unequip(Player player, Item context = null)
    {
        Item toUnequip = context ?? this;

        player.Damage.Value -= Damage;
        player.Luck.Value -= 7;

        player.Inventory.Items.Add(toUnequip);
        player.LeftHand = null;
        player.RightHand = null;
    }
}

