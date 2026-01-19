using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        // Initialize logging
        Logger.Initialize(LogLevel.Debug);
        Logger.Info("GameManager initialized", "GameManager");

        // Set up global exception handling
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        Logger.Info("Game started successfully", "GameManager");
    }

    public override void _ExitTree()
    {
        Logger.Info("Game shutting down", "GameManager");
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Logger.Exception(ex, "UnhandledException");
        }
        else
        {
            Logger.Error($"Unhandled exception: {e.ExceptionObject}", "UnhandledException");
        }
    }

    public static void SafeExecute(Action action, string context = null)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, context);
        }
    }

    public static T SafeExecute<T>(Func<T> func, T defaultValue = default, string context = null)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, context);
            return defaultValue;
        }
    }
}
