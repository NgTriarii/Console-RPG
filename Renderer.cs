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

	public void DrawGameOver()
	{
		Console.Clear();
		Console.SetCursorPosition(0,0);
		Console.Write("Game over...");
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
		string rightHand = $"Right Hand : {(player.RightHand != null ? player.RightHand.Name : ' ')}";
		Console.Write(rightHand.Substring(0, (invWidth > rightHand.Length) ? rightHand.Length : invWidth) + ((invWidth < rightHand.Length) ? "..." : " ") );
		Console.SetCursorPosition(mapWidth, currHeight++);
		string leftHand = $"Left Hand  : {(player.LeftHand != null ? player.LeftHand.Name : ' ')}";
        Console.Write(leftHand.Substring(0, (invWidth > leftHand.Length) ? leftHand.Length : invWidth) + ((invWidth < leftHand.Length) ? "..." : " "));
		Console.SetCursorPosition(mapWidth, currHeight++);
		Console.Write($"Coins: {player.Inventory.Coins} Gold: {player.Inventory.Gold}");

		Stat[] statsToDraw = new Stat[]
		{
			player.Health,
			player.Damage,
			player.Strength,
			player.Dexterity,
			player.Wisdom,
			player.Luck,
			player.Aggression
		};

		for (int i = 0; i < statsToDraw.Length; i++)
		{
			Console.SetCursorPosition(mapWidth, currHeight++);
			Console.Write($"{statsToDraw[i].Name} - {statsToDraw[i].Value}");
		}

		Console.SetCursorPosition(mapWidth, currHeight++);
		Console.Write("Items:");

		for (int i = 0; i < player.Inventory.Count; i++)
		{
			Console.SetCursorPosition(mapWidth, currHeight++);

			string? item = $"-{player.Inventory.Items[i].Name} {(cursorPos == i ? '<' : ' ')}";

            Console.Write(item.Substring(0, (invWidth > item.Length) ? item.Length : invWidth));
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

		Console.SetCursorPosition(0, mapHeight + 4);
		Console.Write($"Attack Mode: {player.CurrentAttack.Name}");
	}

	private void DrawMessageAndControls(int mapWidth, string message, InputHandler inputChain)
	{
		int startX = mapWidth + 40;

        if (startX >= Console.BufferWidth) return;

		int maxSafeWidth = Console.BufferWidth - startX - 20;

		if (maxSafeWidth <= 5) return;

        if (!string.IsNullOrEmpty(message))
        {
            message = message.Replace("\n", " ").Replace("\r", "");
        }
        else
        {
            message = "";
        }

        string blankLine = new string(' ', maxSafeWidth);

        Console.SetCursorPosition(startX, 0);
        Console.Write("----------Messages----------".PadRight(maxSafeWidth));

        int currentY = 1;

        for (int i = 0; i < message.Length; i += maxSafeWidth)
        {
            int remainingLength = message.Length - i;
            int lengthToTake = Math.Min(maxSafeWidth, remainingLength);

            string lineChunk = message.Substring(i, lengthToTake);

            Console.SetCursorPosition(startX, currentY);

            Console.Write(lineChunk.PadRight(maxSafeWidth));

            currentY++;
        }

        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string(' ', maxSafeWidth));
        currentY++;

        Console.SetCursorPosition(startX, currentY);
        Console.Write("----------Controls----------".PadRight(maxSafeWidth));
		currentY++;

        List<string> controls = new List<string>();
		inputChain.CollectHelpText(controls);

		foreach (var ctrl in controls)
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

	public void DrawLog(LogManager logger, int mapwidth, int mapheight, int listlength) {

		Console.SetCursorPosition(mapwidth + 20, mapheight - 1);
		Console.Write("-----------Log-----------");
		List<string> messages = logger.GetRecent(listlength);
		for(int i = 0; i < messages.Count; i++)
		{
            Console.SetCursorPosition(mapwidth + 20, mapheight + i);
            Console.Write(messages[i].PadRight(48).Substring(0, 48));
		}
	}
}

