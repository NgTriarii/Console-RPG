using OOD_Project.Entities;
using OOD_Project.Inputs;
using OOD_Project.Logging;
using OOD_Project.WorldGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project;

// Helper class to set up the game world
public static class GameInitializer
{
    public static GameConfig LoadOrCreateConfig(string filePath)
    {
        var config = new GameConfig();

        if (!File.Exists(filePath))
        {
            string[] defaultLines =
            {
                "[Settings]",
                $"PlayerName={config.PlayerName}",
                $"LogFilePath={config.LogFilePath}"
            };
            File.WriteAllLines(filePath, defaultLines);
            return config;
        }

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("[")) continue;

            string[] parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (key.Equals("PlayerName", StringComparison.OrdinalIgnoreCase))
                {
                    config.PlayerName = value;
                }
                else if (key.Equals("LogFilePath", StringComparison.OrdinalIgnoreCase))
                {
                    config.LogFilePath = value;
                }
            }
        }

        return config;
    }

    public static void InitLogging(GameConfig config)
    {
        ILogger fileLogger = new FileLogger(config.PlayerName, config.LogFilePath);
        ILogger memoryLogger = new MemoryLogger(100);
        LogManager.Instance.Initialize(fileLogger, memoryLogger);
        LogManager.Instance.Log($"Game started by {config.PlayerName}.");
    }

    // Create the map and spawn enemies
    public static (GameModel model, InputHandler inputChain) Build(GameConfig config)
    {
        IMapBuilder mapBuilder = new StandardMapBuilder();
        IInputBuilder inputBuilder = new InputChainBuilder();
        GameDirector director = new GameDirector(mapBuilder, inputBuilder);
        IDungeonTheme theme = new TreasuryTheme();

        director.ConstructThemedDungeon(theme, 50, 20);

        Map map = mapBuilder.GetResult();
        InputHandler inputChain = inputBuilder.GetResult();
        List<Enemy> enemies = mapBuilder.SpawnedEnemies;
        SoundManager soundManager = new SoundManager(map);

        InitializeEnemyFactions(enemies, soundManager);

        GameModel model = new GameModel(map, enemies, soundManager, config)
        {
            CurrentMessage = theme.IntroMessage
        };

        return (model, inputChain);
    }

    private static void InitializeEnemyFactions(List<Enemy> enemies, SoundManager soundManager)
    {
        var goblinFaction = new Species();
        var briefcaseFaction = new Species();
        var safeboxmimicFaction = new Species();

        foreach (var enemy in enemies)
        {
            ISubject<DeathEvent> assignedFaction;
            if (enemy is Goblin) assignedFaction = goblinFaction;
            else if (enemy is BriefcaseBrawler) assignedFaction = briefcaseFaction;
            else if (enemy is SafeboxMimic) assignedFaction = safeboxmimicFaction;
            else assignedFaction = new Species();
            enemy.RegisterObservers(soundManager, assignedFaction);
        }
    }
}
