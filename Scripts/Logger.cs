using Godot;
using System;
using System.IO;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

public static class Logger
{
    private static LogLevel _minLevel = LogLevel.Debug;
    private static string _logFilePath;
    private static readonly object _lock = new object();

    public static void Initialize(LogLevel minLevel = LogLevel.Debug, string logFileName = "gemma.log")
    {
        _minLevel = minLevel;
        _logFilePath = Path.Combine(OS.GetUserDataDir(), logFileName);

        try
        {
            // Create or clear log file
            File.WriteAllText(_logFilePath, $"=== Gemma Log Started {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n");
            Info("Logger initialized");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to initialize log file: {ex.Message}");
        }
    }

    public static void Debug(string message, string context = null)
    {
        Log(LogLevel.Debug, message, context);
    }

    public static void Info(string message, string context = null)
    {
        Log(LogLevel.Info, message, context);
    }

    public static void Warn(string message, string context = null)
    {
        Log(LogLevel.Warning, message, context);
    }

    public static void Error(string message, string context = null)
    {
        Log(LogLevel.Error, message, context);
    }

    public static void Exception(Exception ex, string context = null)
    {
        var message = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
        Log(LogLevel.Error, message, context);
    }

    private static void Log(LogLevel level, string message, string context)
    {
        if (level < _minLevel)
            return;

        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var levelStr = level.ToString().ToUpper().PadRight(7);
        var contextStr = string.IsNullOrEmpty(context) ? "" : $"[{context}] ";
        var logLine = $"[{timestamp}] {levelStr} {contextStr}{message}";

        // Print to Godot console
        switch (level)
        {
            case LogLevel.Error:
                GD.PrintErr(logLine);
                break;
            case LogLevel.Warning:
                GD.Print($"WARNING: {logLine}");
                break;
            default:
                GD.Print(logLine);
                break;
        }

        // Write to file
        WriteToFile(logLine);
    }

    private static void WriteToFile(string line)
    {
        if (string.IsNullOrEmpty(_logFilePath))
            return;

        lock (_lock)
        {
            try
            {
                File.AppendAllText(_logFilePath, line + "\n");
            }
            catch
            {
                // Silently fail file writes to avoid cascading errors
            }
        }
    }
}
