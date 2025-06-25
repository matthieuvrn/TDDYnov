using Notification.Contracts;

namespace Notification.Services;

public class Logger : ILogger
{
    private readonly List<string> _logs = new();

    public void LogInfo(string message)
    {
        var logEntry = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        _logs.Add(logEntry);
        Console.WriteLine(logEntry);
    }

    public void LogError(string message)
    {
        var logEntry = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        _logs.Add(logEntry);
        Console.WriteLine(logEntry);
    }

    public void LogWarning(string message)
    {
        var logEntry = $"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        _logs.Add(logEntry);
        Console.WriteLine(logEntry);
    }

    // méthode helper pour les tests
    public IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();
    public void ClearLogs() => _logs.Clear();
}