using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Convert keyboard keys to actions to send over the network
public static class KeyMapper
{
    public static ActionType? ToAction(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                return ActionType.MoveUp;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                return ActionType.MoveDown;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                return ActionType.MoveLeft;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                return ActionType.MoveRight;
            case ConsoleKey.E:
                return ActionType.PickUp;
            case ConsoleKey.G:
                return ActionType.Drop;
            case ConsoleKey.R:
                return ActionType.Equip;
            case ConsoleKey.T:
                return ActionType.Unequip;
            case ConsoleKey.U:
                return ActionType.ToggleAttack;
            default:
                return null;
        }
    }
}
