using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine;

public class Renderer
{
    public void DrawFrame(Map map, Player player, int cursorPos, string message, InputHandler inputChain)
    {
        Console.SetCursorPosition(0, 0);
        DrawMap(map, player);
        DrawInventory(player, map.Width, map.Height, cursorPos);
        DrawDescription(map, player, map.Height);
        DrawMessageAndControls(map.Width, message, inputChain);
    }

    private void DrawMap(Map map, Player player)
    {
        for (int y = 0; y < map.Height; y++)
        {
            Console.SetCursorPosition(0, y);
            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < map.Width; x++)
            {
                if (player.X == x && player.Y == y)
                {
                    sb.Append('¶');
                }
                else
                {
                    sb.Append(map.Tiles[x, y].Symbol);
                }
            }
            Console.Write(sb.ToString());
        }
    }

    private void DrawInventory(Player player, int mapWidth, int mapHeight, int cursorPos)
    {
        int invWidth = 10;
        string blankLine = new string(' ', invWidth);

        for (int i = 0; i < mapHeight; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 1);
            Console.Write(blankLine);
        }

        Console.SetCursorPosition(mapWidth, 0);
        Console.Write("----------Inventory----------");
        Console.SetCursorPosition(mapWidth, 1);
        Console.Write($"Right Hand: {(player.RightHand != null ? player.RightHand.Name : ' ')} | Left Hand: {(player.LeftHand != null ? player.LeftHand.Name : ' ')} ");
        Console.SetCursorPosition(mapWidth, 2);
        Console.Write($"Coins: {player.Inventory.Coins} Gold: {player.Inventory.Gold}");

        for (int i = 0; i < player.Stats.Count; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 3);
            Console.Write($"{player.Stats[i].Name} - {player.Stats[i].Value}");
        }

        Console.SetCursorPosition(mapWidth, player.Stats.Count + 3);
        Console.Write("Items:");

        for (int i = 0; i < player.Inventory.Count; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 4 + player.Stats.Count);
            Console.Write($"-{player.Inventory.Items[i].Name} {(cursorPos == i ? '<' : ' ')}");
        }
    }

    private void DrawDescription(Map map, Player player, int mapHeight)
    {
        int safeWidth = Console.WindowWidth - 2;
        if (safeWidth < 1) safeWidth = 50;

        Console.SetCursorPosition(0, mapHeight + 2);
        Console.Write(new string(' ', safeWidth));

        Console.SetCursorPosition(0, mapHeight + 1);
        Console.Write("Tile descrpition:");

        Console.SetCursorPosition(0, mapHeight + 2);
        if (map.Tiles[player.X, player.Y].ItemOnTile != null)
        {
            Console.Write($"{map.Tiles[player.X, player.Y].ItemOnTile.Name} - {map.Tiles[player.X, player.Y].ItemOnTile.Description}");
        }
        else
        {
            Console.Write("An empty tile");
        }
    }

    private void DrawMessageAndControls(int mapWidth, string message, InputHandler inputChain)
    {
        int startX = mapWidth + 35;

        if (startX >= Console.WindowWidth) return;

        int maxSafeWidth = Console.WindowWidth - startX - 3;

        if (maxSafeWidth <= 5) return;

        Console.SetCursorPosition(startX, 0);
        Console.Write(message.PadRight(maxSafeWidth/2));

        Console.SetCursorPosition(startX, 2);
        Console.Write("Available Controls:");

        List<string> controls = new List<string>();
        inputChain.CollectHelpText(controls);

        int lineOffset = 0;
        foreach (var ctrl in controls)
        {
            Console.SetCursorPosition(startX, 3 + lineOffset);
            Console.Write(ctrl);
            lineOffset++;
        }
    }
}

