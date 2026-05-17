using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Entities;
public class Stat
{
    public virtual string Name { get; set; } = "Generic Stat";

    public virtual int Value { get; set; } = 1;
}

internal class Luck : Stat
{
    public override string Name => "Luck";
}

internal class Strength : Stat
{
    public override string Name => "Strength";
}

internal class Dexterity : Stat
{
    public override string Name => "Dexterity";
}

internal class Health : Stat
{
    public override string Name => "Health";

    public Health()
    {
        Value = 100;
    }
}

internal class Aggression : Stat
{
    public override string Name => "Aggression";
}

internal class Wisdom : Stat
{
    public override string Name => "Wisdom";
}

internal class Damage : Stat
{
    public override string Name => "Damage";

    private int _value;

    public override int Value { get { return _value > 0 ? _value : 0; } set {_value = value; } }
}
