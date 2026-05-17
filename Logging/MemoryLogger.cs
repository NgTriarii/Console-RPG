using OOD_Project.Logging;
using System.Collections.Generic;

namespace OOD_Project.Logging;

public class MemoryLogger : ILogger
{
    private readonly List<string> _logs = new List<string>();
    private readonly int _maxCapacity;

    public MemoryLogger(int maxCapacity = 100)
    {
        _maxCapacity = maxCapacity;
    }

    public void Log(string message)
    {
        _logs.Add(message);

        if (_logs.Count > _maxCapacity)
        {
            _logs.RemoveAt(0);
        }
    }

    public List<string> GetHistory() => new List<string>(_logs);

    public List<string> GetRecent(int count)
    {
        int skipCount = Math.Max(0, _logs.Count - count);
        return _logs.Skip(skipCount).ToList();
    }
}