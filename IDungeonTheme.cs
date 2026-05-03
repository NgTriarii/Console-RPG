using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public interface IDungeonTheme
{
    string IntroMessage { get; }
    Item GetRandomItem(Random rng);
    Enemy GetRandomEnemy(Random rng);
    Item GetArtifact();
    Weapon GetRandomWeapon(Random rng);
    void ApplyGenerationStrategy(IMapBuilder builder);
}
