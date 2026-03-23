using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public class GameDirector
{
    private IMapBuilder _mapBuilder;
    private IInputBuilder _inputBuilder;

    public GameDirector(IMapBuilder mapBuilder, IInputBuilder inputBuilder)
    {
        _mapBuilder = mapBuilder;
        _inputBuilder = inputBuilder;
    }

    // Option 1: Dungeon
    public void ConstructDungeon(int width, int height)
    {
        _mapBuilder.StartFilled(width, height);
        _mapBuilder.AddCentralRoom(10, 5);
        _mapBuilder.AddPaths();
        _mapBuilder.AddChambers();
        _mapBuilder.AddItems(5);
        _mapBuilder.AddWeapons(2);

        _inputBuilder.StartBuilding();
        _inputBuilder.AddMovement();

        if (_mapBuilder.HasItemsAdded || _mapBuilder.HasWeaponsAdded)
        {
            _inputBuilder.AddItemInteractions();
        }

        if (_mapBuilder.HasWeaponsAdded)
        {
            _inputBuilder.AddEquipmentInteractions();
        }

        _inputBuilder.AddSystemInteractions();
    }

    // Option 2: Empty room
    public void ConstructEmptyRoom(int width, int height)
    {
        _mapBuilder.StartEmpty(width, height);

        _inputBuilder.StartBuilding();
        _inputBuilder.AddMovement();
        _inputBuilder.AddSystemInteractions();
    }
}
