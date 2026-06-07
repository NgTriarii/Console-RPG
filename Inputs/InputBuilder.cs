using OOD_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Inputs;

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
        AppendToChain(new SimpleActionHandler(ConsoleKey.E, g => new PickUpCommand().Execute(g.Model, g.LocalPlayer), "Pick Up Item"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.G, g => new DropCommand(g.CursorPos).Execute(g.Model, g.LocalPlayer), "Drop Item"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.I, g => g.MoveCursor(), "Move Cursor"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.U, g => new ToggleAttackCommand().Execute(g.Model, g.LocalPlayer), "Change Attack Type"));
    }

    public void AddEquipmentInteractions()
    {
        AppendToChain(new SimpleActionHandler(ConsoleKey.R, g => new EquipCommand(g.CursorPos).Execute(g.Model, g.LocalPlayer), "Equip Item"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.T, g => new UnequipCommand().Execute(g.Model, g.LocalPlayer), "Unequip Item"));
    }

    public void AddSystemInteractions()
    {
        AppendToChain(new SimpleActionHandler(ConsoleKey.Escape, g => Environment.Exit(0), "Exit Game"));
        AppendToChain(new SimpleActionHandler(ConsoleKey.J, g => g.ToggleLog(), "Show Log"));
    }

    public InputHandler GetResult() => _head;
}
