using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Draws the game state for connected clients
public class NetworkClientView
{
    private readonly List<string> _log = new List<string>();
    private const int LogCapacity = 100;

    // Available controls
    private static readonly List<string> Controls = new List<string>
    {
        "W/A/S/D or Arrows - Move",
        "E - Pick Up Item",
        "G - Drop Item",
        "I - Move Cursor",
        "U - Change Attack Type",
        "R - Equip Item",
        "T - Unequip Item",
        "J - Show Log",
        "Escape - Exit Game"
    };

    public void Initialize()
    {
        int requiredWidth = 140;
        int requiredHeight = 90;

        if (Console.BufferWidth < requiredWidth) Console.BufferWidth = requiredWidth;
        if (Console.WindowWidth < requiredWidth) Console.WindowWidth = requiredWidth;
        if (Console.BufferHeight < requiredHeight) Console.BufferHeight = requiredHeight;
        if (Console.WindowHeight < requiredHeight) Console.WindowHeight = requiredHeight;

        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Log(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        _log.Add(message);
        if (_log.Count > LogCapacity) _log.RemoveAt(0);
    }

    public void Render(GameStateDto state, int cursorPos, bool showLog)
    {
        Console.SetCursorPosition(0, 0);
        DrawMap(state);
        DrawInventory(state, cursorPos);
        DrawDescription(state);
        DrawMessageAndControls(state.Width, state.Message);

        if (showLog)
        {
            DrawLog(state.Width, state.Height);
        }
    }

    public void ShowGameOver()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.Write("Game over... press any key to exit.");
    }

    public void ShowDisconnected()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.Write("Disconnected from server. Press any key to exit.");
    }

    private void DrawMap(GameStateDto s)
    {
        // Draw the map and everything on it
        var itemsAt = new HashSet<(int, int)>();
        foreach (var it in s.Items) itemsAt.Add((it.X, it.Y));

        var enemiesAt = new Dictionary<(int, int), char>();
        foreach (var e in s.Enemies) enemiesAt[(e.X, e.Y)] = e.Symbol;

        var playersAt = new Dictionary<(int, int), char>();
        foreach (var p in s.Players) playersAt[(p.X, p.Y)] = (char)('0' + p.Id);

        for (int y = 0; y < s.Height; y++)
        {
            Console.SetCursorPosition(0, y);
            var sb = new StringBuilder(s.Width);
            string row = (y < s.Terrain.Length) ? s.Terrain[y] : string.Empty;

            for (int x = 0; x < s.Width; x++)
            {
                char c = (x < row.Length) ? row[x] : ' ';
                if (itemsAt.Contains((x, y))) c = 'i';
                if (enemiesAt.TryGetValue((x, y), out char ec)) c = ec;
                if (playersAt.TryGetValue((x, y), out char pc)) c = pc;
                sb.Append(c);
            }
            Console.Write(sb.ToString());
        }
    }

    private void DrawInventory(GameStateDto s, int cursorPos)
    {
        LocalPlayerDto you = s.You;
        int mapWidth = s.Width;
        int mapHeight = s.Height;
        int invWidth = 35;
        int currHeight = 0;
        string blankLine = new string(' ', invWidth);

        for (int i = 0; i < mapHeight; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 1);
            Console.Write(blankLine);
        }

        Console.SetCursorPosition(mapWidth, currHeight++);
        Console.Write("-------------Inventory-------------");

        Console.SetCursorPosition(mapWidth, currHeight++);
        Console.Write(Fit($"Right Hand : {(you.RightHand ?? " ")}", invWidth));

        Console.SetCursorPosition(mapWidth, currHeight++);
        Console.Write(Fit($"Left Hand  : {(you.LeftHand ?? " ")}", invWidth));

        Console.SetCursorPosition(mapWidth, currHeight++);
        Console.Write($"Coins: {you.Coins} Gold: {you.Gold}");

        (string Name, int Value)[] stats =
        {
            ("Health", you.Health),
            ("Damage", you.Damage),
            ("Strength", you.Strength),
            ("Dexterity", you.Dexterity),
            ("Wisdom", you.Wisdom),
            ("Luck", you.Luck),
            ("Aggression", you.Aggression)
        };

        foreach (var (name, value) in stats)
        {
            Console.SetCursorPosition(mapWidth, currHeight++);
            Console.Write($"{name} - {value}");
        }

        Console.SetCursorPosition(mapWidth, currHeight++);
        Console.Write("Items:");

        for (int i = 0; i < you.Inventory.Count; i++)
        {
            Console.SetCursorPosition(mapWidth, currHeight++);
            string item = $"-{you.Inventory[i]} {(cursorPos == i ? '<' : ' ')}";
            Console.Write(Fit(item, invWidth));
        }
    }

    private void DrawDescription(GameStateDto s)
    {
        LocalPlayerDto you = s.You;
        int mapHeight = s.Height;

        int safeWidth = Console.WindowWidth - 2;
        if (safeWidth < 1) safeWidth = 50;

        Console.SetCursorPosition(0, mapHeight + 2);
        Console.Write(new string(' ', safeWidth));

        Console.SetCursorPosition(0, mapHeight + 1);
        Console.Write("Tile descrpition:");

        Console.SetCursorPosition(0, mapHeight + 2);
        GroundItemDto? here = s.Items.Find(it => it.X == you.X && it.Y == you.Y);
        if (here != null)
        {
            Console.Write($"{here.Name} - {here.Description}");
        }
        else
        {
            Console.Write("An empty tile");
        }

        Console.SetCursorPosition(0, mapHeight + 4);
        Console.Write($"Attack Mode: {you.AttackMode}");
    }

    private void DrawMessageAndControls(int mapWidth, string message)
    {
        int startX = mapWidth + 40;
        if (startX >= Console.BufferWidth) return;

        int maxSafeWidth = Console.BufferWidth - startX - 20;
        if (maxSafeWidth <= 5) return;

        message = string.IsNullOrEmpty(message) ? "" : message.Replace("\n", " ").Replace("\r", "");

        Console.SetCursorPosition(startX, 0);
        Console.Write("----------Messages----------".PadRight(maxSafeWidth));

        int currentY = 1;
        for (int i = 0; i < message.Length; i += maxSafeWidth)
        {
            int lengthToTake = Math.Min(maxSafeWidth, message.Length - i);
            Console.SetCursorPosition(startX, currentY);
            Console.Write(message.Substring(i, lengthToTake).PadRight(maxSafeWidth));
            currentY++;
        }

        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string(' ', maxSafeWidth));
        currentY++;

        Console.SetCursorPosition(startX, currentY);
        Console.Write("----------Controls----------".PadRight(maxSafeWidth));
        currentY++;

        foreach (var ctrl in Controls)
        {
            Console.SetCursorPosition(startX, currentY);
            string safeCtrl = ctrl.Length > maxSafeWidth ? ctrl.Substring(0, maxSafeWidth) : ctrl;
            Console.Write(safeCtrl.PadRight(maxSafeWidth));
            currentY++;
        }

        for (int i = currentY; i <= 15; i++)
        {
            Console.SetCursorPosition(startX, i);
            Console.Write(new string(' ', maxSafeWidth));
        }
    }

    private void DrawLog(int mapWidth, int mapHeight)
    {
        Console.SetCursorPosition(mapWidth + 20, mapHeight - 1);
        Console.Write("-----------Log-----------");

        int listLength = 7;
        int skip = Math.Max(0, _log.Count - listLength);
        for (int i = 0; i + skip < _log.Count; i++)
        {
            Console.SetCursorPosition(mapWidth + 20, mapHeight + i);
            Console.Write(_log[skip + i].PadRight(48).Substring(0, 48));
        }
    }

    private static string Fit(string text, int width) =>
        text.Length > width ? text.Substring(0, width) : text;
}
