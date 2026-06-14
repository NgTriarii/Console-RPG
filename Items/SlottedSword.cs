using OOD_Project.Entities;
using System.Collections.Generic;

namespace OOD_Project.Items;

public class SlottedSword : LightWeapon, IItemContainer
{
    public List<Item> Slots { get; } = new List<Item>();
    public int MaxSlots { get; }

    public SlottedSword(int maxSlots) 
    { 
        MaxSlots = maxSlots; 
    }

    public override string Name => $"Slotted Sword ({Slots.Count}/{MaxSlots} slots)";
    public override int Damage => 5;

    public bool TryAdd(Item item)
    {
        if (item.IsSlottable)
        {
            if (Slots.Count < MaxSlots) 
            { 
                Slots.Add(item); 
                return true; 
            }
        }
        return false;
    }

    public Item ExtractLast()
    {
        if (Slots.Count > 0)
        {
            Item item = Slots[Slots.Count - 1];
            Slots.RemoveAt(Slots.Count - 1);
            return item;
        }
        return null;
    }

    public override int Equip(Player player, Item context = null)
    {
        if (context == null)
        {
            if (player.RightHand == null) 
            {
                player.Damage.Value += Damage;
                player.Strength.Value += 2; 

                player.Inventory.Items.Remove(this);
                player.RightHand = this;

                foreach(var item in Slots) item.Equip(player, this);

                return 1;
            }
            return 0;
        }
        else
        {
            player.Damage.Value += Damage;
            player.Strength.Value += 2; 

            foreach(var item in Slots) item.Equip(player, context);

            return 1;
        }
    }

    public override void Unequip(Player player, Item context = null)
    {
        if (context == null)
        {
            player.Inventory.Items.Add(this);
            player.RightHand = null;
        }

        player.Damage.Value -= Damage;
        player.Strength.Value -= 2;

        foreach(var item in Slots) item.Unequip(player, context ?? this);
    }
}
