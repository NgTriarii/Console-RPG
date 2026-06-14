using OOD_Project.Entities;
using OOD_Project.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project;

// Basic game settings
public class GameConfig
{
    public string PlayerName { get; set; } = "Hero";
    public string LogFilePath { get; set; } = "Logs";
}

// Stores the current game data (map, enemies, players)
public class GameModel
{
    public const int MaxPlayers = 9;

    public Map GameMap { get; }
    public List<Enemy> ActiveEnemies { get; }
    public SoundManager SoundManager { get; }
    public GameConfig Config { get; }

    // Dictionary of all players currently in the game
    public Dictionary<int, Player> Players { get; } = new Dictionary<int, Player>();

    public string CurrentMessage { get; set; } = "Welcome to the game!";
    public bool IsGameOver { get; set; } = false;

    public GameModel(Map map, List<Enemy> enemies, SoundManager soundManager, GameConfig config)
    {
        GameMap = map;
        ActiveEnemies = enemies;
        SoundManager = soundManager;
        Config = config;
    }

    // Add a new player and give them an ID
    public int AddPlayer(Player player)
    {
        for (int id = 1; id <= MaxPlayers; id++)
        {
            if (!Players.ContainsKey(id))
            {
                player.Id = id;
                Players[id] = player;
                SoundManager.Attach(player);   // so the player can hear nearby sounds
                return id;
            }
        }
        return -1;
    }

    public void RemovePlayer(int id)
    {
        if (Players.TryGetValue(id, out Player? player))
        {
            SoundManager.Detach(player);
            Players.Remove(id);
        }
    }

    private readonly Random _spawnRng = new Random();

    // Find an empty spot to spawn a new player
    public (int X, int Y) FindSpawnPoint()
    {
        for (int attempt = 0; attempt < 1000; attempt++)
        {
            int x = _spawnRng.Next(1, GameMap.Width - 1);
            int y = _spawnRng.Next(1, GameMap.Height - 1);

            if (GameMap.IsValidMove(x, y) && GetPlayerAt(x, y) == null)
            {
                return (x, y);
            }
        }
        return (GameMap.Width / 2, GameMap.Height / 2);
    }

    // Check if there's a player standing on a specific tile
    public Player? GetPlayerAt(int x, int y)
    {
        foreach (var player in Players.Values)
        {
            if (player.X == x && player.Y == y)
            {
                return player;
            }
        }
        return null;
    }

    // Remove dead enemies and let the rest take their turn
    public void AdvanceEnemyTurn()
    {
        ActiveEnemies.RemoveAll(e => e.IsDead);

        foreach (var enemy in ActiveEnemies)
        {
            enemy.CurrentBehaviour.Act(enemy, this);
        }
    }
}
