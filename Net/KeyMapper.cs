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
                return ActionType.Equip;
            case ConsoleKey.Q:
                return ActionType.Drop;
            default:
                return null;
        }
    }
}
