using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;
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

}

public class HorizBorder : Tile
{

    public override char Symbol => '-';

    public override bool IsEnterable => false;
}

public class VertBorder : Tile
{

    public override char Symbol => '|';

    public override bool IsEnterable => false;
}