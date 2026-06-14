using OOD_Project.Entities;

namespace OOD_Project.Items;

public abstract class PassiveItem : Item
{
    public override bool IsEquippable => true;
    public override bool IsSlottable => true;

    protected abstract void ApplyStats(Player player);
    protected abstract void RemoveStats(Player player);

    public override int Equip(Player player, Item context = null)
    {
        // If context is null, it's being equipped directly to the player's hand
        if (context == null)
        {
            if (player.RightHand == null)
            {
                player.RightHand = this;
                player.Inventory.Items.Remove(this);
                ApplyStats(player);
                return 1;
            }
            return 0;
        }
        else
        {
            // If context is not null, it's inside a slot (e.g., inside a Sword or a Holder)
            // We just apply the stats without touching the player's hands.
            ApplyStats(player);
            return 1;
        }
    }

    public override void Unequip(Player player, Item context = null)
    {
        if (context == null)
        {
            player.RightHand = null;
            player.Inventory.Items.Add(this);
        }
        RemoveStats(player);
    }
}

public class GemOfStrength : PassiveItem
{
    public override string Name => "Gem of Strength";
    protected override void ApplyStats(Player player) => player.Strength.Value += 2;
    protected override void RemoveStats(Player player) => player.Strength.Value -= 2;
}

public class GemOfLuck : PassiveItem
{
    public override string Name => "Gem of Luck";
    protected override void ApplyStats(Player player) => player.Luck.Value += 2;
    protected override void RemoveStats(Player player) => player.Luck.Value -= 2;
}

public class GemOfWisdom : PassiveItem
{
    public override string Name => "Gem of Wisdom";
    protected override void ApplyStats(Player player) => player.Wisdom.Value += 2;
    protected override void RemoveStats(Player player) => player.Wisdom.Value -= 2;
}
