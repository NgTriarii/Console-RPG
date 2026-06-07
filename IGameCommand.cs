using OOD_Project.Entities;
using OOD_Project.Items;
using OOD_Project.Logging;
using OOD_Project.WorldGeneration;
using System;

namespace OOD_Project;

// An action that changes the game state for a specific player
public interface IGameCommand
{
    void Execute(GameModel model, Player player);
}

public class MoveCommand : IGameCommand
{
    private readonly int _dx;
    private readonly int _dy;

    public MoveCommand(int dx, int dy)
    {
        _dx = dx;
        _dy = dy;
    }

    public void ResolveCombat(Tile enemyTile, Player player, GameModel model)
    {
        Enemy enemy = enemyTile.EnemyOnTile;

        IAttackAction chosenAttack = player.CurrentAttack;

        Item equippedWeapon = player.RightHand ?? new BareHands();

        CombatStats stats = equippedWeapon.AcceptAttack(chosenAttack, player);

        enemy.TakeDamage(stats.Damage);

        if (enemy.IsDead)
        {
            enemyTile.EnemyOnTile = null;
            player.LastMessage = $"You defeated the {enemy.Name}!";
            return;
        }
        else
        {
            player.LastMessage = $"You dealt {stats.Damage} damage to {enemy.Name}! (Current Health: {enemy.Health}";
            LogManager.Instance.Log($"Player dealt {stats.Damage} damage to {enemy.Name}! (Current Health: {enemy.Health}");
        }

        int damageToPlayer = Math.Max(0, enemy.Attack - stats.Defense);

        if (damageToPlayer > 0)
        {
            player.TakeDamage(damageToPlayer);
        }
    }

    public void Execute(GameModel model, Player player)
    {
        int nextX = player.X + _dx;
        int nextY = player.Y + _dy;

        Tile targetTile = model.GameMap.Tiles[nextX, nextY];

        if (targetTile.EnemyOnTile != null)
        {
            ResolveCombat(targetTile, player, model);
        }

        else if (model.GetPlayerAt(nextX, nextY) != null)
        {
            LogManager.Instance.Log($"{player.Name} bumped into another player.");
        }

        else if (!targetTile.IsEnterable)
        {
            LogManager.Instance.Log("Player attempted to walk into a wall");
        }

        else
        {
            player.Move(_dx, _dy, model.GameMap.Width, model.GameMap.Height);
            model.SoundManager.Notify(new SoundEvent(player.X, player.Y, 4, player.Name, model.GameMap));
            targetTile.OnEntry(player);
        }
    }
}

public class DropCommand : IGameCommand
{
    private readonly int _slotIndex;

    public DropCommand(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    public void Execute(GameModel model, Player player)
    {
        Tile currentTile = model.GameMap.Tiles[player.X, player.Y];

        if (currentTile.ItemOnTile == null)
        {
            if (player.Inventory.Count == 0)
            {
                player.LastMessage = "Your inventory is empty!";
                return;
            }

            Item droppedItem = player.Inventory.Items[_slotIndex];

            if (droppedItem.SoundRange > 0)
            {
                model.SoundManager.Notify(new SoundEvent(player.X, player.Y, droppedItem.SoundRange, droppedItem.Name, model.GameMap));
            }

            player.LastMessage = $"Dropped an item: {droppedItem.Name}";
            currentTile.ItemOnTile = droppedItem;
            player.DropItem(_slotIndex);
        }
        else
        {
            player.LastMessage = "There's already an item on the ground here.";
        }
    }
}

public class ToggleAttackCommand : IGameCommand
{
    public void Execute(GameModel model, Player player)
    {
        player.ToggleAttackMode();
        player.LastMessage = $"Attack mode changed to: {player.CurrentAttack.Name}";
    }
}

public class PickUpCommand : IGameCommand
{
    public void Execute(GameModel model, Player player)
    {
        Tile currentTile = model.GameMap.Tiles[player.X, player.Y];

        if (currentTile.ItemOnTile != null)
        {
            if (player.Inventory.Count == player.Inventory.Limit)
            {
                player.LastMessage = $"Cannot pick up {currentTile.ItemOnTile.Name} - inventory full.";
                return;
            }

            if (currentTile.ItemOnTile.SoundRange > 0)
            {
                model.SoundManager.Notify(new SoundEvent(player.X, player.Y, currentTile.ItemOnTile.SoundRange, currentTile.ItemOnTile.Name, model.GameMap));
            }

            // The item determines what happens when picked up (e.g., gold goes to wallet, swords go to inventory)
            currentTile.ItemOnTile.OnPickUp(player);
            player.LastMessage = $"Picked up {currentTile.ItemOnTile.Name}.";

            // Remove it from the map
            currentTile.ItemOnTile = null;
        }
        else
        {
            player.LastMessage = "Nothing here to pick up.";
        }
    }
}

public class EquipCommand : IGameCommand
{
    private readonly int _slotIndex;

    public EquipCommand(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    public void Execute(GameModel model, Player player)
    {
        if (player.Inventory.Count == 0)
        {
            player.LastMessage = "Your inventory is empty!";
            return;
        }

        if (_slotIndex < player.Inventory.Count)
        {
            Item selectedItem = player.Inventory.Items[_slotIndex];

            int success = selectedItem.Equip(player);

            player.LastMessage = ((selectedItem.IsEquippable && success == 1) ? $"Equipped {selectedItem.Name}." : $"Couldn't equip {selectedItem.Name}.");
        }
    }
}

public class UnequipCommand : IGameCommand
{
    public void Execute(GameModel model, Player player)
    {
        bool unequippedAnything = false;

        if (player.RightHand != null)
        {
            player.RightHand.Unequip(player);
            unequippedAnything = true;
        }

        if (player.LeftHand != null)
        {
            player.LeftHand.Unequip(player);
            unequippedAnything = true;
        }

        player.LastMessage = unequippedAnything
            ? "Unequipped items."
            : "You don't have anything equipped in your hands.";
    }
}
