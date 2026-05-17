using OOD_Project.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEngine;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    public FileLogger(string playerName, string baseDirectory)
    {
        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }

        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{playerName}_Log_{timeStamp}.txt";

        _filePath = Path.Combine(baseDirectory, fileName);
    }

    public void Log(string message)
    {
        string formattedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
        File.AppendAllText(_filePath, formattedMessage);
    }
    public List<string> GetHistory() => new List<string>();

    public List<string> GetRecent(int count) => new List<string>();
}