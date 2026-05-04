using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public interface IGameCommand
{
    void Execute(Game game);
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

    public void ResolveCombat(Tile enemyTile, Player player, Game game)
    {
        Enemy enemy = enemyTile.EnemyOnTile;

        IAttackAction chosenAttack = player.CurrentAttack;

        Item equippedWeapon = player.RightHand ?? new BareHands();

        CombatStats stats = equippedWeapon.AcceptAttack(chosenAttack, player);

        enemy.TakeDamage(stats.Damage);

        if (enemy.IsDead)
        {
            enemyTile.EnemyOnTile = null;
            game.CurrentMessage = $"You defeated the {enemy.Name}!";
            return;
        }
        else
        {
            game.CurrentMessage = $"You dealt {stats.Damage} damage to {enemy.Name}! (Current Health: {enemy.Health}";
            LogManager.Instance.Log($"Player dealt {stats.Damage} damage to {enemy.Name}! (Current Health: {enemy.Health}");
        }

            int damageToPlayer = Math.Max(0, enemy.Attack - stats.Defense);

        if (damageToPlayer > 0)
        {
            player.TakeDamage(damageToPlayer);
        }

    }

    public void Execute(Game game)
    {
        int nextX = game.GamePlayer.X + _dx;
        int nextY = game.GamePlayer.Y + _dy;

        Tile targetTile = game.GameMap.Tiles[nextX, nextY];

        if (targetTile.EnemyOnTile != null)
        {
            ResolveCombat(targetTile, game.GamePlayer, game);
        }

        else if (!targetTile.IsEnterable)
        {
            LogManager.Instance.Log("Player attempted to walk into a wall");
        }


        else if (targetTile.IsEnterable)
        {
            game.GamePlayer.Move(_dx, _dy, game.GameMap.Width, game.GameMap.Height);
            targetTile.OnEntry(game.GamePlayer);
        }
    }
}

public class DropCommand : IGameCommand
{
    public void Execute(Game game)
    {
        Tile currentTile = game.GameMap.Tiles[game.GamePlayer.X, game.GamePlayer.Y];

        if (currentTile.ItemOnTile == null)
        {
            if (game.GamePlayer.Inventory.Count == 0)
            {
                game.CurrentMessage = "Your inventory is empty!";
                return;
            }

            Player player = game.GamePlayer;

            game.CurrentMessage = $"Dropped an item: {player.Inventory.Items[game.CursorPos].Name}";
            game.GameMap.Tiles[player.X, player.Y].ItemOnTile = player.Inventory.Items[game.CursorPos];
            player.DropItem(game.CursorPos);


        }
        else
        {
            game.CurrentMessage = "There's already an item on the ground here.";
        }
    }
}

public class ToggleAttackCommand : IGameCommand
{
    public void Execute(Game game)
    {
        game.GamePlayer.ToggleAttackMode();
        game.CurrentMessage = $"Attack mode changed to: {game.GamePlayer.CurrentAttack.Name}";
    }
}
public class PickUpCommand : IGameCommand
{
    public void Execute(Game game)
    {
        Tile currentTile = game.GameMap.Tiles[game.GamePlayer.X, game.GamePlayer.Y];

        if (currentTile.ItemOnTile != null)
        {
            if(game.GamePlayer.Inventory.Count == game.GamePlayer.Inventory.Limit)
            {
                game.CurrentMessage = $"Cannot pick up {currentTile.ItemOnTile.Name} - inventory full.";
                return;
            }
            // The item determines what happens when picked up (e.g., gold goes to wallet, swords go to inventory)
            currentTile.ItemOnTile.OnPickUp(game.GamePlayer);
            game.CurrentMessage = $"Picked up {currentTile.ItemOnTile.Name}.";

            // Remove it from the map
            currentTile.ItemOnTile = null;
        }
        else
        {
            game.CurrentMessage = "Nothing here to pick up.";
        }
    }
}

public class EquipCommand : IGameCommand
{
    public void Execute(Game game)
    {
        if (game.GamePlayer.Inventory.Count == 0)
        {
            game.CurrentMessage = "Your inventory is empty!";
            return;
        }

        if (game.CursorPos < game.GamePlayer.Inventory.Count)
        {
            Item selectedItem = game.GamePlayer.Inventory.Items[game.CursorPos];

            int success = 0;

            success = selectedItem.Equip(game.GamePlayer);

            game.CurrentMessage = ((selectedItem.IsEquippable && success == 1) ? $"Equipped {selectedItem.Name}." : $"Couldn't equip {selectedItem.Name}.");
        }
    }
}

public class UnequipCommand : IGameCommand
{
    public void Execute(Game game)
    {
        bool unequippedAnything = false;

        if (game.GamePlayer.RightHand != null)
        {
            game.GamePlayer.RightHand.Unequip(game.GamePlayer);
            unequippedAnything = true;
        }

        if (game.GamePlayer.LeftHand != null)
        {
            game.GamePlayer.LeftHand.Unequip(game.GamePlayer);
            unequippedAnything = true;
        }

        if (unequippedAnything)
        {
            game.CurrentMessage = "Unequipped items.";
        }
        else
        {
            game.CurrentMessage = "You don't have anything equipped in your hands.";
        }
    }
}

public class ShowLogCommand : IGameCommand
{
    public void Execute(Game game)
    {
        game.isLogShown = !game.isLogShown;
    }
}