using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public class TreasuryTheme : IDungeonTheme
{
    public string IntroMessage => "You feel an itch in your wallet.";

    private readonly Func<Item>[] _lootTable = { () => new Gold(), () => new Coin() };
    private readonly Func<Enemy>[] _enemyTable = { () => new SafeboxMimic(), () => new BriefcaseBrawler() };
    private readonly Func<Weapon>[] _weaponTable = { () => new Zweihander(), () => new Rapier() };

    public Item GetRandomItem(Random rng) => _lootTable[rng.Next(_lootTable.Length)]();
    public Enemy GetRandomEnemy(Random rng) => _enemyTable[rng.Next(_enemyTable.Length)]();
    public Weapon GetRandomWeapon(Random rng) => _weaponTable[rng.Next(_weaponTable.Length)]();
    public Item GetArtifact() => new LuckyCoinPouch();

    public void ApplyGenerationStrategy(IMapBuilder builder)
    {
        builder.StartFilled(50, 20);
        builder.AddCentralRoom(20, 10);
        builder.AddChambers();
        builder.AddPaths();
    }
}
