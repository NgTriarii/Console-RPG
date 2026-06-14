using OOD_Project.Entities;
using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.WorldGeneration;

public interface IMapBuilder
{
    void StartEmpty(int width, int height);
    void StartFilled(int width, int height);


    void AddPaths();
    void AddChambers();
    void AddCentralRoom(int roomWidth, int roomHeight);
    void AddLoot(int totalItems, IDungeonTheme theme);
    void AddWeapons(int totalWeapons, IDungeonTheme theme);
    void AddEnemies(int totalEnemies, IDungeonTheme theme);
    void AddModifiedWeapons(int totalItems, IDungeonTheme theme);
    void AddSlotItems();
    void PlaceSpecificItem(Item item);


    Map GetResult();
    bool HasItemsAdded { get; }
    bool HasWeaponsAdded { get; }
    List<Enemy> SpawnedEnemies { get; }
}
