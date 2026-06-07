using System;
using System.Collections.Generic;

namespace OOD_Project.Logging;

public interface ILogger
{
    void Log(string message);
    List<string> GetRecent(int count);
    List<string> GetHistory();
}
