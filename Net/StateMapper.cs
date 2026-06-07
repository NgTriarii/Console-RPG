using OOD_Project.Entities;
using OOD_Project.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Converts the game state into DTOs
public static class StateMapper
{
    public static GameStateDto ToDto(GameModel model, int forPlayerId)
    {
        Map map = model.GameMap;

        var dto = new GameStateDto
        {
            Width = map.Width,
            Height = map.Height,
            IsGameOver = model.IsGameOver
        };

        // Create the map view
        var terrain = new string[map.Height];
        for (int y = 0; y < map.Height; y++)
        {
            var sb = new StringBuilder(map.Width);
            for (int x = 0; x < map.Width; x++)
            {
                Tile tile = map.Tiles[x, y];
                sb.Append((tile.IsWall || tile.IsBorder) ? tile.Symbol : ' ');

                if (tile.ItemOnTile != null)
                {
                    dto.Items.Add(new GroundItemDto
                    {
                        X = x,
                        Y = y,
                        Name = tile.ItemOnTile.Name,
                        Description = tile.ItemOnTile.Description
                    });
                }
            }
            terrain[y] = sb.ToString();
        }
        dto.Terrain = terrain;

        foreach (var enemy in model.ActiveEnemies)
        {
            dto.Enemies.Add(new EnemyDto { X = enemy.X, Y = enemy.Y, Symbol = enemy.Symbol });
        }

        foreach (var kvp in model.Players)
        {
            dto.Players.Add(new PlayerDto { Id = kvp.Key, X = kvp.Value.X, Y = kvp.Value.Y });
        }

        if (model.Players.TryGetValue(forPlayerId, out Player? me))
        {
            dto.You = BuildLocalPlayer(me);
            dto.Message = me.LastMessage;
        }

        return dto;
    }

    private static LocalPlayerDto BuildLocalPlayer(Player p)
    {
        var dto = new LocalPlayerDto
        {
            Id = p.Id,
            Name = p.Name ?? "",
            X = p.X,
            Y = p.Y,
            Health = p.Health.Value,
            Damage = p.Damage.Value,
            Strength = p.Strength.Value,
            Dexterity = p.Dexterity.Value,
            Wisdom = p.Wisdom.Value,
            Luck = p.Luck.Value,
            Aggression = p.Aggression.Value,
            RightHand = p.RightHand?.Name,
            LeftHand = p.LeftHand?.Name,
            Coins = p.Inventory.Coins,
            Gold = p.Inventory.Gold,
            AttackMode = p.CurrentAttack.Name
        };

        foreach (var item in p.Inventory.Items)
        {
            dto.Inventory.Add(item.Name);
        }

        return dto;
    }
}
