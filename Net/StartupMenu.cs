using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Net;

public enum LaunchMode { Server, Client, Local, Quit }

// Stores user launch options
public class LaunchOptions
{
    public LaunchMode Mode { get; }
    public string Host { get; }
    public int Port { get; }

    public LaunchOptions(LaunchMode mode, string host, int port)
    {
        Mode = mode;
        Host = host;
        Port = port;
    }
}

// Initial game menu
public static class StartupMenu
{
    public static LaunchOptions Prompt()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Console RPG - Multiplayer ===");
            Console.WriteLine("  [S] Server  (host a game)");
            Console.WriteLine("  [C] Client  (join a game)");
            Console.WriteLine("  [L] Local   (single-player)");
            Console.WriteLine("  [Q] Quit");
            Console.Write("Choice: ");

            ConsoleKey key = Console.ReadKey(false).Key;
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.S:
                    return new LaunchOptions(LaunchMode.Server, "", PromptPort(5555));

                case ConsoleKey.C:
                    var (host, port) = PromptAddress("127.0.0.1", 5555);
                    return new LaunchOptions(LaunchMode.Client, host, port);

                case ConsoleKey.L:
                    return new LaunchOptions(LaunchMode.Local, "", 0);

                case ConsoleKey.Q:
                    return new LaunchOptions(LaunchMode.Quit, "", 0);

                default:
                    Console.WriteLine("Unrecognized choice, try again.");
                    break;
            }
        }
    }

    private static int PromptPort(int def)
    {
        Console.Write($"Port (Default:[{def}]): ");
        string? line = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(line) && int.TryParse(line.Trim(), out int p))
        {
            return p;
        }
        return def;
    }

    private static (string host, int port) PromptAddress(string defHost, int defPort)
    {
        Console.Write($"Server address (Default - [{defHost}:{defPort}]): ");
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
        {
            return (defHost, defPort);
        }

        string[] parts = line.Trim().Split(':');
        string host = string.IsNullOrWhiteSpace(parts[0]) ? defHost : parts[0];
        int port = (parts.Length > 1 && int.TryParse(parts[1], out int p)) ? p : defPort;
        return (host, port);
    }
}
