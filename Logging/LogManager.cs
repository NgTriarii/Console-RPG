using System.Collections.Generic;

namespace OOD_Project.Logging;

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

    // Lock object to prevent threads from writing to the log at the same time
    private readonly object _lock = new object();

    public void Initialize(ILogger fileLogger, ILogger memoryLogger)
    {
        lock (_lock)
        {
            _fileLogger = fileLogger;
            _memoryLogger = memoryLogger;
        }
    }

    public void Log(string message)
    {
        lock (_lock)
        {
            _fileLogger?.Log(message);
            _memoryLogger?.Log(message);
        }
    }

    public List<string> GetRecent(int count)
    {
        lock (_lock)
        {
            return _memoryLogger?.GetRecent(count) ?? new List<string>();
        }
    }

    public List<string> GetHistory()
    {
        lock (_lock)
        {
            return _memoryLogger?.GetHistory() ?? new List<string>();
        }
    }
}
