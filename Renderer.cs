using System;
using System.Text;

namespace GameEngine;

public class Renderer
{
    public void DrawFrame(Map map, Player player, int cursorPos)
    {
        Console.SetCursorPosition(0, 0);
        DrawMap(map, player);
        DrawInventory(player, map.Width, map.Height, cursorPos);
        DrawDescription(map, player, map.Height);
    }

    private void DrawMap(Map map, Player player)
    {
        Console.SetCursorPosition(0, 0);
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < map.Height; y++)
        {
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
            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }

    private void DrawInventory(Player player, int mapWidth, int mapHeight, int cursorPos)
    {
        for (int i = 0; i < mapHeight; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 1);
            Console.WriteLine("                                                         ");
        }

        Console.SetCursorPosition(mapWidth, 0);
        Console.WriteLine("----------Inventory----------");
        Console.SetCursorPosition(mapWidth, 1);
        Console.WriteLine($"Right Hand: {(player.RightHand != null ? player.RightHand.Name : ' ')} | Left Hand: {(player.LeftHand != null ? player.LeftHand.Name : ' ')} ");
        Console.SetCursorPosition(mapWidth, 2);
        Console.WriteLine($"Coins: {player.Inventory.Coins} Gold: {player.Inventory.Gold}");

        for (int i = 0; i < player.Stats.Count; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 3);
            Console.WriteLine($"{player.Stats[i].Name} - {player.Stats[i].Value}");
        }

        Console.SetCursorPosition(mapWidth, player.Stats.Count + 3);
        Console.WriteLine("Items:");

        for (int i = 0; i < player.Inventory.Count; i++)
        {
            Console.SetCursorPosition(mapWidth, i + 4 + player.Stats.Count);
            Console.WriteLine($"-{player.Inventory.Items[i].Name} {(cursorPos == i ? '<' : ' ')}");
        }

        Console.SetCursorPosition(mapWidth, mapHeight - 1);
        Console.WriteLine("-----------------------------");
    }

    private void DrawDescription(Map map, Player player, int mapHeight)
    {
        Console.SetCursorPosition(0, mapHeight + 2);
        Console.WriteLine("                                                                                                                ");

        Console.SetCursorPosition(0, mapHeight + 1);
        Console.WriteLine("Tile descrpition:");

        if (map.Tiles[player.X, player.Y].ItemOnTile != null)
        {
            Console.WriteLine($"{map.Tiles[player.X, player.Y].ItemOnTile.Name} - {map.Tiles[player.X, player.Y].ItemOnTile.Description}");
        }
        else
        {
            Console.WriteLine("An empty tile");
        }
    }
}