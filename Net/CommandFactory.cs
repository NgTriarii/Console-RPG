using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Convert an action message from a client into a game command
public static class CommandFactory
{
    public static IGameCommand? FromAction(ActionMessage msg)
    {
        switch (msg.Action)
        {
            case ActionType.MoveUp: return new MoveCommand(0, -1);
            case ActionType.MoveDown: return new MoveCommand(0, 1);
            case ActionType.MoveLeft: return new MoveCommand(-1, 0);
            case ActionType.MoveRight: return new MoveCommand(1, 0);
            case ActionType.PickUp: return new PickUpCommand();
            case ActionType.Drop: return new DropCommand(msg.Slot);
            case ActionType.Equip: return new EquipCommand(msg.Slot);
            case ActionType.Unequip: return new UnequipCommand();
            case ActionType.ToggleAttack: return new ToggleAttackCommand();
            default: return null;
        }
    }
}
