using System;
using System.Linq;
using OOD_Project;
using OOD_Project.Net;
using OOD_Project.Entities;
using OOD_Project.Inputs;

class Program
{
    static int Main(string[] args)
    {
        // Launch flags
        int serverIdx = Array.IndexOf(args, "--server");
        if (serverIdx >= 0) { LaunchServer(ParsePort(args, serverIdx, 5555)); return 0; }

        int clientIdx = Array.IndexOf(args, "--client");
        if (clientIdx >= 0)
        {
            var (host, port) = ParseAddress(args, clientIdx, "127.0.0.1", 5555);
            LaunchClient(host, port);
            return 0;
        }

        if (args.Contains("--local")) { LaunchLocal(); return 0; }

        // No flag: interactive menu
        LaunchOptions choice = StartupMenu.Prompt();
        switch (choice.Mode)
        {
            case LaunchMode.Server: LaunchServer(choice.Port); break;
            case LaunchMode.Client: LaunchClient(choice.Host, choice.Port); break;
            case LaunchMode.Local: LaunchLocal(); break;
            case LaunchMode.Quit: break;
        }
        return 0;
    }

    // Launchers

    private static void LaunchLocal() => new Game().Run();

    private static void LaunchClient(string host, int port) => new GameClient(host, port).Run();

    private static void LaunchServer(int port)
    {
        GameConfig config = GameInitializer.LoadOrCreateConfig("config.ini");
        GameInitializer.InitLogging(config);

        var (model, inputChain) = GameInitializer.Build(config);

        // The host is player 1 and plays on the local console
        var (hostX, hostY) = model.FindSpawnPoint();
        Player host = new Player
        {
            Name = config.PlayerName,
            X = hostX,
            Y = hostY,
            LastMessage = model.CurrentMessage
        };
        model.AddPlayer(host);

        Game hostGame = new Game(model, host, inputChain, new ConsoleView(), new ConsoleInputSource());
        new GameServer(hostGame, port).Run();
    }

    // Argument helpers

    private static int ParsePort(string[] args, int idx, int def)
    {
        if (idx + 1 < args.Length && int.TryParse(args[idx + 1], out int p)) return p;
        return def;
    }

    private static (string host, int port) ParseAddress(string[] args, int idx, string defHost, int defPort)
    {
        if (idx + 1 < args.Length && !args[idx + 1].StartsWith("--"))
        {
            string[] parts = args[idx + 1].Split(':');
            string host = string.IsNullOrWhiteSpace(parts[0]) ? defHost : parts[0];
            int port = (parts.Length > 1 && int.TryParse(parts[1], out int p)) ? p : defPort;
            return (host, port);
        }
        return (defHost, defPort);
    }
}
