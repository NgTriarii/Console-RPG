using OOD_Project;
using System;
using System.Collections.Generic;

namespace OOD_Project.Inputs;

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
            case ConsoleKey.W: case ConsoleKey.UpArrow: new MoveCommand(0, -1).Execute(game.Model, game.LocalPlayer); return true;
            case ConsoleKey.S: case ConsoleKey.DownArrow: new MoveCommand(0, 1).Execute(game.Model, game.LocalPlayer); return true;
            case ConsoleKey.A: case ConsoleKey.LeftArrow: new MoveCommand(-1, 0).Execute(game.Model, game.LocalPlayer); return true;
            case ConsoleKey.D: case ConsoleKey.RightArrow: new MoveCommand(1, 0).Execute(game.Model, game.LocalPlayer); return true;
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
