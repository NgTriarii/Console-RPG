using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Simple classes used to send the game state over the network
public class GameStateDto
{
    public int Width { get; set; }
    public int Height { get; set; }

    // The map layout
public string[] Terrain { get; set; } = new string[0];

    public List<EnemyDto> Enemies { get; set; } = new List<EnemyDto>();
    public List<GroundItemDto> Items { get; set; } = new List<GroundItemDto>();
    public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();

    public LocalPlayerDto You { get; set; } = new LocalPlayerDto();

    public string Message { get; set; } = "";
    public bool IsGameOver { get; set; }
}

public class PlayerDto
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public class EnemyDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Symbol { get; set; }
}

public class GroundItemDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}

// Holds player information to show on the UI
public class LocalPlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }

    public int Health { get; set; }
    public int Damage { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Wisdom { get; set; }
    public int Luck { get; set; }
    public int Aggression { get; set; }
    public string? RightHand { get; set; }
    public string? LeftHand { get; set; }

    public int Coins { get; set; }
    public int Gold { get; set; }
    public string AttackMode { get; set; } = "";
    public List<string> Inventory { get; set; } = new List<string>();
}
