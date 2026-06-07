using OOD_Project.Entities;
using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.WorldGeneration;

public interface IDungeonTheme
{
    string IntroMessage { get; }
    Item GetRandomItem(Random rng);
    Enemy GetRandomEnemy(Random rng);
    Item GetArtifact();
    Weapon GetRandomWeapon(Random rng);
    void ApplyGenerationStrategy(IMapBuilder builder);
}
