using GameEngine;
using System;
using System.Collections.Generic;

namespace OOD_Project.Inputs;

public static class InputFactory
{
    public static InputHandler BuildKeyboardChain()
    {
        var root = new MoveHandler();

        root.SetNext(new SimpleActionHandler(ConsoleKey.E, g => new PickUpCommand().Execute(g), "Pick Up Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.I, g =>
            {
                if (g.GamePlayer.Inventory.Count > 0)
                    g.CursorPos = (g.CursorPos + 1) % g.GamePlayer.Inventory.Count;
            }, "Move Inventory Cursor"))
            .SetNext(new SimpleActionHandler(ConsoleKey.R, g => new EquipCommand().Execute(g), "Equip Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.T, g => new UnequipCommand().Execute(g), "Unequip Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.G, g => new DropCommand().Execute(g), "Drop Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.Escape, g => Environment.Exit(0), "Exit Game"))
            .SetNext(new SimpleActionHandler(ConsoleKey.J, g => new ShowLogCommand().Execute(g), "Show Log"));

        return root;
    }
}

public abstract class InputHandler
{
    protected InputHandler? Next;

    public InputHandler SetNext(InputHandler nextHandler)
    {
        Next = nextHandler;
        return nextHandler;
    }
    
    public abstract bool Handle(ConsoleKey key, Game game);

    public abstract void CollectHelpText(List<string> helpText);

}

public class MoveHandler : InputHandler
{
    public override bool Handle(ConsoleKey key, Game game)
    {
        switch (key)
        {
            case ConsoleKey.W: case ConsoleKey.UpArrow: new MoveCommand(0, -1).Execute(game); return true;
            case ConsoleKey.S: case ConsoleKey.DownArrow: new MoveCommand(0, 1).Execute(game); return true;
            case ConsoleKey.A: case ConsoleKey.LeftArrow: new MoveCommand(-1, 0).Execute(game); return true;
            case ConsoleKey.D: case ConsoleKey.RightArrow: new MoveCommand(1, 0).Execute(game); return true;
        }

        return Next?.Handle(key, game) ?? false;
    }

    public override void CollectHelpText(List<string> helpText)
    {
        helpText.Add("W/A/S/D or Arrows - Move");
        Next?.CollectHelpText(helpText);
    }
}

public class SimpleActionHandler : InputHandler
{
    private readonly ConsoleKey _key;
    private readonly Action<Game> _action;
    private readonly string _description;

    public SimpleActionHandler(ConsoleKey key, Action<Game> action, string description)
    {
        _key = key;
        _action = action;
        _description = description;
    }

    public override bool Handle(ConsoleKey key, Game game)
    {
        if (key == _key)
        {
            _action(game);
            return true;
        }

        return Next?.Handle(key, game) ?? false;
    }

    public override void CollectHelpText(List<string> helpText)
    {
        helpText.Add($"{_key} - {_description}");
        Next?.CollectHelpText(helpText);
    }
}