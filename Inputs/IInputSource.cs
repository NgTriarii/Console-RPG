using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project.Inputs;

// Interface for getting input (keyboard or network)
public interface IInputSource
{
    ConsoleKey ReadKey();
}

public class ConsoleInputSource : IInputSource
{
    public ConsoleKey ReadKey() => Console.ReadKey(true).Key;
}
