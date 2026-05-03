using System.Collections.Generic;

namespace GameEngine;

public class LogManager
{
    private static LogManager _instance;

    private LogManager()
    {
    }

    public static LogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LogManager();
            }

            return _instance;
        }
    }

    private ILogger _fileLogger;
    private ILogger _memoryLogger;

    public void Initialize(ILogger fileLogger, ILogger memoryLogger)
    {
        _fileLogger = fileLogger;
        _memoryLogger = memoryLogger;
    }

    public void Log(string message)
    {
        _fileLogger?.Log(message);
        _memoryLogger?.Log(message);
    }

    public List<string> GetRecent(int count) =>
        _memoryLogger?.GetRecent(count) ?? new List<string>();

    public List<string> GetHistory() =>
        _memoryLogger?.GetHistory() ?? new List<string>();
}