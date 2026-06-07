using OOD_Project.Entities;
using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project;
public struct CombatStats
{
    public int Damage;
    public int Defense;
}
public interface IAttackAction
{
    string Name { get; }
    CombatStats ExecuteWithHeavy(Item context, Player player);
    CombatStats ExecuteWithLight(Item context, Player player);
    CombatStats ExecuteWithMagical(Item context, Player player);
    CombatStats ExecuteWithOther(Item context, Player player);
}

public class NormalAttack : IAttackAction
{
    public string Name => "Normal Attack";
    public CombatStats ExecuteWithHeavy(Item context, Player player) => new CombatStats
    {
        Damage = player.Damage.Value,
        Defense = player.Strength.Value + player.Luck.Value
    };

    public CombatStats ExecuteWithLight(Item context, Player player) => new CombatStats
    {
        Damage = player.Damage.Value,
        Defense = player.Dexterity.Value + player.Luck.Value
    };

    public CombatStats ExecuteWithMagical(Item context, Player player) => new CombatStats
    {
        Damage = 1,
        Defense = player.Dexterity.Value + player.Luck.Value
    };

    public CombatStats ExecuteWithOther(Item context, Player player) => new CombatStats
    {
        Damage = 0,
        Defense = player.Dexterity.Value
    };
}

public class StealthAttack : IAttackAction
{
    public string Name => "Stealth Attack";
    public CombatStats ExecuteWithHeavy(Item context, Player player) => new CombatStats
    {
        Damage = player.Damage.Value / 2, // Halved
        Defense = player.Strength.Value
    };

    public CombatStats ExecuteWithLight(Item context, Player player) => new CombatStats
    {
        Damage = player.Damage.Value * 2, // Doubled
        Defense = player.Dexterity.Value
    };

    public CombatStats ExecuteWithMagical(Item context, Player player) => new CombatStats
    {
        Damage = 1,
        Defense = 0
    };

    public CombatStats ExecuteWithOther(Item context, Player player) => new CombatStats
    {
        Damage = 0,
        Defense = 0
    };
}

public class MagicalAttack : IAttackAction
{
    public string Name => "Magical Attack";
    public CombatStats ExecuteWithHeavy(Item context, Player player) => new CombatStats
    {
        Damage = 1,
        Defense = player.Luck.Value
    };

    public CombatStats ExecuteWithLight(Item context, Player player) => new CombatStats
    {
        Damage = 1,
        Defense = player.Luck.Value
    };

    public CombatStats ExecuteWithMagical(Item context, Player player) => new CombatStats
    {
        Damage = player.Damage.Value,
        Defense = player.Wisdom.Value * 2
    };

    public CombatStats ExecuteWithOther(Item context, Player player) => new CombatStats
    {
        Damage = 0,
        Defense = player.Luck.Value
    };
}
