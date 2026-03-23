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

    public void Execute(Game game)
    {
        int nextX = game.GamePlayer.X + _dx;
        int nextY = game.GamePlayer.Y + _dy;

        if (game.GameMap.Tiles[nextX, nextY].IsEnterable)
        {
            game.GamePlayer.Move(_dx, _dy, game.GameMap.Width, game.GameMap.Height);
            game.GameMap.Tiles[game.GamePlayer.X, game.GamePlayer.Y].OnEntry(game.GamePlayer);
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

            game.CurrentMessage = "DROP ITEM: Press a number (0-9) to select slot, or Esc to cancel.";
            game.GameRenderer.DrawFrame(game.GameMap, game.GamePlayer, game.CursorPos, game.CurrentMessage, game.InputChain);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                game.CurrentMessage = "Drop cancelled.";
                return;
            }

            if (char.IsDigit(keyInfo.KeyChar))
            {
                int index = int.Parse(keyInfo.KeyChar.ToString());
                if (index < game.GamePlayer.Inventory.Count)
                {
                    Item dropped = game.GamePlayer.DropItem(index);
                    currentTile.ItemOnTile = dropped;
                    game.CurrentMessage = $"Dropped {dropped.Name}.";

                    if (game.CursorPos >= game.GamePlayer.Inventory.Count)
                        game.CursorPos = Math.Max(0, game.GamePlayer.Inventory.Count - 1);
                }
                else
                {
                    game.CurrentMessage = "Invalid slot number. Drop cancelled.";
                }
            }
            else
            {
                game.CurrentMessage = "Invalid input. Drop cancelled.";
            }
        }
        else
        {
            game.CurrentMessage = "There's already an item on the ground here.";
        }
    }
}

public class PickUpCommand : IGameCommand
{
    public void Execute(Game game)
    {
        Tile currentTile = game.GameMap.Tiles[game.GamePlayer.X, game.GamePlayer.Y];

        if (currentTile.ItemOnTile != null)
        {
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

            // Rely on the Item's virtual Equip method to handle the logic
            selectedItem.Equip(game.GamePlayer);
            game.CurrentMessage = $"Equipped {selectedItem.Name}.";
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