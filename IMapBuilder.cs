using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public interface IMapBuilder
{
    void StartEmpty(int width, int height);
    void StartFilled(int width, int height);


    void AddPaths();
    void AddChambers();
    void AddCentralRoom(int roomWidth, int roomHeight);
    void AddLoot(int totalItems);
    void AddWeapons(int totalWeapons);
    void AddEnemies(int totalEnemies);
    void AddModifiedLoot(int totalItems);


    Map GetResult();
    bool HasItemsAdded { get; }
    bool HasWeaponsAdded { get; }
}