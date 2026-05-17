using OOD_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public class SoundEvent
{
    public int OriginX { get; }
    public int OriginY { get; }
    public int Range { get; }
    public string SourceName { get; }

    public SoundEvent(int originX, int originY, int range, string sourceName)
    {
        OriginX = originX;
        OriginY = originY;
        Range = range;
        SourceName = sourceName;
    }
}

public class DeathEvent
{
    public Enemy DeceasedEnemy { get; }

    public DeathEvent(Enemy deceasedEnemy)
    {
        DeceasedEnemy = deceasedEnemy;
    }
}