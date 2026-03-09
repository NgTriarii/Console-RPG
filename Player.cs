using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;
public class Player
{
    public int X { get; set; } = 1;
    public int Y { get; set; } = 1;

    public Inventory Inventory { get; set; } = new Inventory();

    public List<Stat> Stats = new List<Stat>();

    public Item? RightHand { get; set; }

    public Item? LeftHand { get; set; }

    public Player()
    {
        Stats.Add(new Health());
        Stats.Add(new Aggression());
        Stats.Add(new Wisdom());
        Stats.Add(new Luck());
        Stats.Add(new Strength());
        Stats.Add(new Dexterity());
    }

    public void Move(int dx, int dy, int mapWidth, int mapHeight)
    {

        X = X + dx;
        Y = Y + dy;

    }

    public Item DropItem(int cursor)
    {
        Item DroppedItem = Inventory.Items[cursor];
        Inventory.Items.RemoveAt(cursor);
        return DroppedItem;
    }
}