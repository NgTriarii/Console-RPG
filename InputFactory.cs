using System;

namespace GameEngine;

public static class InputFactory
{
    public static InputHandler BuildKeyboardChain()
    {
        var root = new MoveHandler(); // We'll update MoveHandler slightly below

        root.SetNext(new SimpleActionHandler(ConsoleKey.E, g => new PickUpCommand().Execute(g), "Pick Up Item"))
            // Note: If you want an inline command for simple things like moving the cursor, you can still do it!
            .SetNext(new SimpleActionHandler(ConsoleKey.I, g =>
            {
                if (g.GamePlayer.Inventory.Count > 0)
                    g.CursorPos = (g.CursorPos + 1) % g.GamePlayer.Inventory.Count;
            }, "Move Inventory Cursor"))
            .SetNext(new SimpleActionHandler(ConsoleKey.R, g => new EquipCommand().Execute(g), "Equip Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.T, g => new UnequipCommand().Execute(g), "Unequip Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.G, g => new DropCommand().Execute(g), "Drop Item"))
            .SetNext(new SimpleActionHandler(ConsoleKey.Escape, g => Environment.Exit(0), "Exit Game"));

        return root;
    }
}