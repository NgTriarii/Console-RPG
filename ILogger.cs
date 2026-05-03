using System;
using System.Collections.Generic;

namespace GameEngine;

public interface ILogger
{
    void Log(string message);
    List<string> GetRecent(int count);
    List<string> GetHistory();
}