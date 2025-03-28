using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// A class to handle logging with different categories and severity levels.
/// </summary>
public class Logger {
    /// <summary>
    /// Enum for log categories.
    /// </summary>
    public enum LogCategory {
        EventBus,
        AbilitySystem,
        EnemySpawner,
        Pickups,
        Saving
    }

    /// <summary>
    /// Enum for log severity levels.
    /// </summary>
    public enum LogSeverity {
        Info,
        Warning,
        Error,
        Severe
    }

    private static readonly Dictionary<LogCategory, bool> logSettings = new Dictionary<LogCategory, bool> {
        { LogCategory.EventBus, false },
        { LogCategory.AbilitySystem, true },
        { LogCategory.EnemySpawner, false },
        { LogCategory.Pickups, false },
        { LogCategory.Saving, false }
    };

    private static readonly HashSet<LogSeverity> activeSeverities = new HashSet<LogSeverity> {
        LogSeverity.Info,
        LogSeverity.Warning,
        LogSeverity.Error,
        LogSeverity.Severe
    };

    private static string logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

    /// <summary>
    /// Sets the logging status of a specific category.
    /// </summary>
    /// <param name="category">The log category.</param>
    /// <param name="isEnabled">If set to <c>true</c> logging is enabled for the category.</param>
    public static void SetLogCategory(LogCategory category, bool isEnabled) {
        if (logSettings.ContainsKey(category)) {
            logSettings[category] = isEnabled;
        } else {
            Debug.LogWarning("Unknown log category: " + category);
        }
    }

    /// <summary>
    /// Sets the logging status of a specific severity level.
    /// </summary>
    /// <param name="severity">The log severity.</param>
    /// <param name="isEnabled">If set to <c>true</c> logging is enabled for the severity level.</param>
    public static void SetLogSeverity(LogSeverity severity, bool isEnabled) {
        if (isEnabled) {
            activeSeverities.Add(severity);
        } else {
            activeSeverities.Remove(severity);
        }
    }

    /// <summary>
    /// Logs a message if the category and severity are enabled.
    /// </summary>
    /// <param name="log">The log message.</param>
    /// <param name="category">The log category.</param>
    /// <param name="severity">The log severity.</param>
    public static void Log(string log, LogCategory category, LogSeverity severity = LogSeverity.Info) {
        if (logSettings.TryGetValue(category, out bool isEnabled) && isEnabled && activeSeverities.Contains(severity)) {
            string formattedLog = $"[{severity}] : [{category}] : {log}";
            Debug.Log(formattedLog);

            WriteToFile(formattedLog);
        }
    }

    static void WriteToFile(string log) {
        try {
            using StreamWriter writer = new(logFilePath, true);
            writer.WriteLine($"[{DateTime.Now}] {log}");
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to write to log file: {ex.Message}");
        }
    }
}