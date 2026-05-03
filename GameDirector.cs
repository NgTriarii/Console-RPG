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

    public void ConstructThemedDungeon(IDungeonTheme theme, int width, int height)
    {
        theme.ApplyGenerationStrategy(_mapBuilder);

        _mapBuilder.AddLoot(12, theme);
        _mapBuilder.AddEnemies(4, theme);
        _mapBuilder.AddModifiedWeapons(5, theme);

        _mapBuilder.PlaceSpecificItem(theme.GetArtifact());

        _inputBuilder.StartBuilding();
        _inputBuilder.AddMovement();

        if (_mapBuilder.HasItemsAdded || _mapBuilder.HasWeaponsAdded)
            _inputBuilder.AddItemInteractions();

        if (_mapBuilder.HasWeaponsAdded)
            _inputBuilder.AddEquipmentInteractions();

        _inputBuilder.AddSystemInteractions();
    }

    // Option 1: Dungeon
    //public void ConstructDungeon(int width, int height)
    //{
    //    _mapBuilder.StartFilled(width, height);
    //    _mapBuilder.AddCentralRoom(10, 5);
    //    _mapBuilder.AddChambers();
    //    //_mapBuilder.AddCentralRoom(10, 5);
    //    _mapBuilder.AddPaths();
    //    _mapBuilder.AddLoot(12);
    //    _mapBuilder.AddEnemies(4);
    //    _mapBuilder.AddModifiedLoot(3);
    //    _inputBuilder.StartBuilding();
    //    _inputBuilder.AddMovement();

    //    if (_mapBuilder.HasItemsAdded || _mapBuilder.HasWeaponsAdded)
    //    {
    //        _inputBuilder.AddItemInteractions();
    //    }

    //    if (_mapBuilder.HasWeaponsAdded)
    //    {
    //        _inputBuilder.AddEquipmentInteractions();
    //    }

    //    _inputBuilder.AddSystemInteractions();
    //}

    // Option 2: Empty room
    public void ConstructEmptyRoom(int width, int height)
    {
        _mapBuilder.StartEmpty(width, height);

        _inputBuilder.StartBuilding();
        _inputBuilder.AddMovement();
        _inputBuilder.AddSystemInteractions();
    }
}
