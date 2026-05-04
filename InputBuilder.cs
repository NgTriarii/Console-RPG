using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public interface IInputBuilder
{
    void StartBuilding();
    void AddMovement();
    void AddItemInteractions();
    void AddEquipmentInteractions();
    void AddSystemInteractions();
    InputHandler GetResult();
}

public class InputChainBuilder : IInputBuilder
{
    private InputHandler _head;
    private InputHandler _currentTail;

    public void StartBuilding()
    {
        _head = null;
        _currentTail = null;
    }

    private void AppendToChain(InputHandler handler)
    {
        if (_head == null)
        {
            _head = handler;
            _currentTail = handler;
        }
        else
        {
            _currentTail = _currentTail.SetNext(handler);
        }
    }

    public void AddMovement()
    {
        AppendToChain(new MoveHandler());
    }

    public void AddItemInteractions()
    {
        AppendToChain(new SimpleActionHandler(ConsoleKey.E, g => new PickUpCommand().Execute(g), "Pick Up Item"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.G, g => new DropCommand().Execute(g), "Drop Item"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.I, g =>
        {
            if (g.GamePlayer.Inventory.Count > 0)
                g.CursorPos = (g.CursorPos + 1) % g.GamePlayer.Inventory.Count;
        }, "Move Cursor"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.U, g => new ToggleAttackCommand().Execute(g), "Change Attack Type"));
    }

    public void AddEquipmentInteractions()
    {
        AppendToChain(new SimpleActionHandler(ConsoleKey.R, g => new EquipCommand().Execute(g), "Equip Item"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.T, g => new UnequipCommand().Execute(g), "Unequip Item"));
    }

    public void AddSystemInteractions()
    {
        AppendToChain(new SimpleActionHandler(ConsoleKey.Escape, g => Environment.Exit(0), "Exit Game"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.J, g => new ShowLogCommand().Execute(g), "Show Log"));
    }

    public InputHandler GetResult() => _head;
}
