using OOD_Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Entities;
public class Inventory
{
    public int Limit => 10;

    public int Count => Items.Count;

    public int Gold { get; set; } = 0;

    public int Coins { get; set; } = 0;

    public List<Item> Items = new List<Item>();
}
