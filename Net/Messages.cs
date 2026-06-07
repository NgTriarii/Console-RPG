using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Net;



// Actions a player can take
public enum ActionType
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    PickUp,
    Drop,
    Equip,
    Unequip,
    ToggleAttack
}

public class ActionMessage
{
    public ActionType Action { get; set; }

    // Target slot for inventory actions
    public int Slot { get; set; }
}



public enum ServerMessageType
{
    Init,   // Initial connection
    State   // Updated game state
}

public class ServerMessage
{
    public ServerMessageType Type { get; set; }

    // Which player this message is for
    public int PlayerId { get; set; }

    public GameStateDto? State { get; set; }
}
