using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;
public class Map
{
    public Tile[,] Tiles;
    public int PlayWidth;
    public int PlayHeight;
    public int Width;
    public int Height;

    public Tile GetTileAt(int x, int y)
    {
        return Tiles[x, y];
    }

    public bool IsValidMove(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            return false;
        }
        else
        {
            if (Tiles[x, y].IsEnterable == false) 
            {
                return false;
            }
        }

        return true;

    }

    public Map(int x, int y)
    {
        PlayHeight = y;
        PlayWidth = x;
        Width = PlayWidth + 2;
        Height = PlayHeight + 2;
        Tiles = new Tile[Width, Height];
    }
}

