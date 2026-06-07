using OOD_Project.Entities;
using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.WorldGeneration;
public class Tile
{
    public virtual char Symbol
    {
        get
        {
            if (EnemyOnTile != null) return EnemyOnTile.Symbol;
            if (ItemOnTile != null) return 'i';
            return ' ';
        }
    }
    public virtual bool IsEnterable => EnemyOnTile == null;
    public virtual bool IsWall => false;
    public virtual bool IsBorder => false;

    public Enemy? EnemyOnTile { get; set; } = null;

    public Item? ItemOnTile { get; set; } = null;
    public virtual void OnEntry(Player player)
    {
    }
}
public class Wall : Tile
{

    public override char Symbol => '█';

    public override bool IsEnterable => false;

    public override bool IsWall => true;

}

public class HorizBorder : Tile
{

    public override char Symbol => '-';

    public override bool IsEnterable => false;

    public override bool IsBorder => true;
}

public class VertBorder : Tile
{

    public override char Symbol => '|';

    public override bool IsEnterable => false;

    public override bool IsBorder => true;
}
